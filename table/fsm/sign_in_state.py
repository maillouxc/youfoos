from datetime import datetime
from time import time

from rfid_listener import RfidListener
from game import Game
from game_state_messenger import GameStartMessage
from fsm import State


class SignInState(State):

    def __init__(self, fsm):
        super(SignInState, self).__init__(fsm)
        self.rfid_listener = RfidListener(uid_listener=self.on_uid_received)
        self._start_button_press_time = None

    def enter(self):
        super(SignInState, self).enter()
        table = self.fsm.table_controller

        # Differentiating between long and short presses is a huge pain...
        table.start_button.when_pressed = self.on_start_button_pressed
        table.start_button.when_released = self.on_start_button_released
        table.start_button.when_held = self.on_start_button_held

        # Start listening for RFID card scans
        table.rfid_card_monitor.addObserver(self.rfid_listener)

    def _if_valid_player_config_turn_on_start_btn_led(self):
        """
        If it is possible to start the game with this setup, light start btn.
        """
        table = self.fsm.table_controller
        if table.game.is_valid_player_config():
            table.start_btn_LED.on()

    def _if_nobody_signed_in_turn_on_start_btn_led(self):
        table = self.fsm.table_controller
        if (table.game.gold_offense_rfid is None
                and table.game.gold_defense_rfid is None
                and table.game.black_offense_rfid is None
                and table.game.black_defense_rfid is None):
            table.start_btn_LED.on()
        else:
            table.start_btn_LED.off()

    def execute(self):
        super(SignInState, self).execute()
        table = self.fsm.table_controller

        self._if_nobody_signed_in_turn_on_start_btn_led()
        self._if_valid_player_config_turn_on_start_btn_led()

        # If the last game was less than one minute ago...
        if table.previous_game is not None and table.previous_game.end_time_utc is not None:
            if (datetime.utcnow() - table.previous_game.end_time_utc).total_seconds() < 60:
                self._allow_signin_with_previous_players()

    # Do not use this for button actions - it is for debouncing
    def on_start_button_pressed(self):
        self._start_button_press_time = time()

    def on_start_button_released(self):
        # This can happen if the button was already down on entering the state
        if self._start_button_press_time is None:
            return

        held_time = time() - self._start_button_press_time

        # Check if it was short press - holds are handled in the other function
        if held_time < self.fsm.table_controller.start_button.hold_time:
            if self.fsm.table_controller.game.is_valid_player_config():
                self.start_game()

    def on_start_button_held(self):
        print("Sign-in cancelled")
        self.fsm.transition("to-idle")

    def exit(self):
        super(SignInState, self).exit()
        table = self.fsm.table_controller

        table.start_button.when_pressed = None
        table.start_button.when_released = None
        table.start_button.when_held = None
        table.start_btn_LED.off()
        self.fsm.table_controller.set_gcb_led_lights(on=False)

        # Stop listening for RFID card scans
        table.rfid_card_monitor.deleteObserver(self.rfid_listener)

    def on_uid_received(self, uid):
        print("RFID read: " + str(uid))
        table = self.fsm.table_controller
        table.last_rfid_read = uid
        table.beeper.beep(on_time=0.4, off_time=0.4, n=1, background=True)
        self.fsm.transition("id-scanned")

    def start_game(self):
        print("Starting game...")

        # Create some shorthand variable names for convenience
        table = self.fsm.table_controller
        game = table.game

        # Shorthand boolean variables denoting which players are signed in
        go = game.gold_offense_rfid is not None
        gd = game.gold_defense_rfid is not None
        bo = game.black_offense_rfid is not None
        bd = game.black_defense_rfid is not None

        same_gold = game.gold_offense_rfid == game.gold_defense_rfid
        same_black = game.black_offense_rfid == game.black_defense_rfid

        # If all 4 players are signed in, gametype is 2v2
        if ((go and gd) and (bo and bd)) and not(same_gold or same_black):
            game_type = Game.GAME_TYPE_2V2
        else:
            game_type = Game.GAME_TYPE_1V1

            # If singles, we need to assign each player's rfid to both spots
            if go:
                game.gold_defense_rfid = game.gold_offense_rfid
            elif gd:
                game.gold_offense_rfid = game.gold_defense_rfid

            if bo:
                game.black_defense_rfid = game.black_offense_rfid
            elif bd:
                game.black_offense_rfid = game.black_defense_rfid

        game.start_time_utc = datetime.utcnow()
        game_start_msg = GameStartMessage(game_guid=game.game_guid,
                                          game_type=game_type,
                                          gold_offense_rfid=game.gold_offense_rfid,
                                          gold_defense_rfid=game.gold_defense_rfid,
                                          black_offense_rfid=game.black_offense_rfid,
                                          black_defense_rfid=game.black_defense_rfid,
                                          timestamp=game.start_time_utc.isoformat())

        try:
            table.rabbitmq_messenger.send_message(game_start_msg)
        except Exception as e:
            print("ERROR: Failed to send game start message")
            print(str(e))
            return

        self.fsm.transition("start-game")

    def _allow_signin_with_previous_players(self):
        self._turn_on_leds_for_all_unclaimed_spots()
        table = self.fsm.table_controller

        # If a GCB is pressed, sign the previous player in at the same spot
        # No need for them to rescan their card again
        if table.gcb_LED_gold_offense.is_lit and table.gcb_gold_offense.is_pressed:
            table.game.gold_offense_rfid = table.previous_game.gold_offense_rfid
            table.gcb_LED_gold_offense.off()
            print("Gold offense spot reclaimed by previous player")
        elif table.gcb_LED_gold_defense.is_lit and table.gcb_gold_defense.is_pressed:
            table.game.gold_defense_rfid = table.previous_game.gold_defense_rfid
            table.gcb_LED_gold_defense.off()
            print("Gold defense spot reclaimed by previous player")
        elif table.gcb_LED_black_offense.is_lit and table.gcb_black_offense.is_pressed:
            table.game.black_offense_rfid = table.previous_game.black_offense_rfid
            table.gcb_LED_black_offense.off()
            print("Black offense spot reclaimed by previous player")
        elif table.gcb_LED_black_defense.is_lit and table.gcb_black_defense.is_pressed:
            table.game.black_defense_rfid = table.previous_game.black_defense_rfid
            table.gcb_LED_black_defense.off()
            print("Black defense spot reclaimed by previous player")
        
        # After they have had a chance to reclaim their spot, reset the lights to off
        # This prevents weirdness with the state of the light not updating properly
        table.set_gcb_led_lights(on=False)

    def _turn_on_leds_for_all_unclaimed_spots(self):
        """
        Turns on the Green start button LEDs for all currently unclaimed spots.
        """
        table = self.fsm.table_controller
        game = table.game
        previous_game = table.previous_game

        if game.gold_offense_rfid is None:
            if not table.is_player_signed_in(previous_game.gold_offense_rfid):
                table.gcb_LED_gold_offense.on()
        if game.black_offense_rfid is None:
            if not table.is_player_signed_in(previous_game.black_offense_rfid):
                table.gcb_LED_black_offense.on()
        if game.gold_defense_rfid is None:
            if not table.is_player_signed_in(previous_game.gold_defense_rfid):
                table.gcb_LED_gold_defense.on()
        if game.black_defense_rfid is None:
            if not table.is_player_signed_in(previous_game.black_defense_rfid):
                table.gcb_LED_black_defense.on()
