from fsm import State


class SpotClaimState(State):

    def __init__(self, fsm):
        super(SpotClaimState, self).__init__(fsm)
        self.was_start_button_held = True

    def enter(self):
        super(SpotClaimState, self).enter()

        # Initialize to true so we can handle if it was already down on entry
        self.was_start_button_held = True

        self._release_spot_if_player_was_already_signed_in()
        self._block_erroneous_fifth_player_signin_attempts()
        self._turn_on_leds_for_all_unclaimed_spots()

    def _release_spot_if_player_was_already_signed_in(self):
        """
        If the player was already signed-in, release their spot -
        this will allow them to choose a new spot to play at.
        If their spot is released and they then hold down the start button
        (which causes the table to return to the idle state), then they
        will have effectively cancelled their sign-in altogether.
        """
        table = self.fsm.table_controller
        if table.last_rfid_read == table.game.gold_offense_rfid:
            print("Releasing gold offense spot")
            table.game.gold_offense_rfid = None
        elif table.last_rfid_read == table.game.gold_defense_rfid:
            print("Releasing gold defense spot")
            table.game.gold_defense_rfid = None
        elif table.last_rfid_read == table.game.black_offense_rfid:
            print("Releasing black offense spot")
            table.game.black_offense_rfid = None
        elif table.last_rfid_read == table.game.black_defense_rfid:
            print("Releasing black defense spot")
            table.game.black_defense_rfid = None

    def _block_erroneous_fifth_player_signin_attempts(self):
        """
        If there are already 4 players signed in and a fifth tries, block it.
        Unless of course it is a player releasing their spot;
        That case would be handled by the above.
        """
        table = self.fsm.table_controller
        if table.get_num_players_signed_in() == 4:
            print('ERROR: 5th player tried to sign in - ignoring')
            self.fsm.transition("to-sign-in")

    def _turn_on_leds_for_all_unclaimed_spots(self):
        """
        Turns on the Green start button LEDs for all currently unclaimed spots.
        """
        table = self.fsm.table_controller

        if table.game.gold_offense_rfid is None:
            table.gcb_LED_gold_offense.on()
        if table.game.gold_defense_rfid is None:
            table.gcb_LED_gold_defense.on()
        if table.game.black_offense_rfid is None:
            table.gcb_LED_black_offense.on()
        if table.game.black_defense_rfid is None:
            table.gcb_LED_black_defense.on()

    def execute(self):
        super(SpotClaimState, self).execute()
        self._claim_spot_if_unclaimed_spot_button_pressed()
        self._return_to_signin_state_if_undo_held()

    def _claim_spot_if_unclaimed_spot_button_pressed(self):
        """
        If an unclaimed spot button is pressed, sign the player in to that spot,
        and transition back to the sign-in state.
        """
        table = self.fsm.table_controller

        if table.gcb_LED_gold_offense.is_lit and table.gcb_gold_offense.is_pressed:
            table.game.gold_offense_rfid = table.last_rfid_read
            print("Gold offense spot claimed")
            self.fsm.transition("to-sign-in")
        elif table.gcb_LED_gold_defense.is_lit and table.gcb_gold_defense.is_pressed:
            table.game.gold_defense_rfid = table.last_rfid_read
            print("Gold defense spot claimed")
            self.fsm.transition("to-sign-in")
        elif table.gcb_LED_black_offense.is_lit and table.gcb_black_offense.is_pressed:
            table.game.black_offense_rfid = table.last_rfid_read
            print("Black offense spot claimed")
            self.fsm.transition("to-sign-in")
        elif table.gcb_LED_black_defense.is_lit and table.gcb_black_defense.is_pressed:
            table.game.black_defense_rfid = table.last_rfid_read
            print("Black defense spot claimed")
            self.fsm.transition("to-sign-in")

    def _return_to_signin_state_if_undo_held(self):
        """
        If the undo button is held down, returns to the sign-in state.
        """
        table = self.fsm.table_controller

        if table.start_button.is_held:
            if not self.was_start_button_held:
                self.fsm.transition("to-sign-in")
            # By setting to true here we prevent dupe undos
            self.was_start_button_held = True
        else:
            self.was_start_button_held = False

    def exit(self):
        super(SpotClaimState, self).exit()
        self.fsm.table_controller.set_gcb_led_lights(on=False)
