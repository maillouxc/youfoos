from datetime import datetime
from time import sleep

from gpiozero import Button, LED, Buzzer
from Adafruit_LED_Backpack import SevenSegment
from smartcard.CardMonitoring import CardMonitor

from game_state_messenger import GoalUndoMessage


class TableController(object):

    def __init__(self, rabbitmq_messenger):
        self.rabbitmq_messenger = rabbitmq_messenger

        self.gold_breakbeam = Button(pin=27)
        self.black_breakbeam = Button(pin=17)
        self.beeper = Buzzer(pin=22)

        self.start_button = Button(pin=25)
        self.gcb_gold_offense = Button(pin=12)
        self.gcb_gold_defense = Button(pin=21)
        self.gcb_black_offense = Button(pin=20)
        self.gcb_black_defense = Button(pin=16)

        self.start_btn_LED = LED(pin=5)
        self.gcb_LED_gold_offense = LED(pin=26)
        self.gcb_LED_gold_defense = LED(pin=6)
        self.gcb_LED_black_offense = LED(pin=13)
        self.gcb_LED_black_defense = LED(pin=19)

        self.score_display = SevenSegment.SevenSegment(address=0x70)
        self.clock_display = SevenSegment.SevenSegment(address=0x71)

        self.rfid_card_monitor = CardMonitor()
        self.last_rfid_read = None

        self.game = None
        self.previous_game = None

    def do_hardware_init(self):
        """
        Run a short test/init sequence on the components, both to notify the user that boot is complete,
        and to verify that all hardware components are working properly.
        """
        print("Running boot sequence...")

        self.start_btn_LED.on()
        self.gcb_LED_gold_offense.on()
        self.gcb_LED_gold_defense.on()
        self.gcb_LED_black_offense.on()
        self.gcb_LED_black_defense.on()

        # Test the 7-segment displays
        self.clock_display.begin()
        self.clock_display.clear()
        self.clock_display.set_colon(True)
        self.clock_display.set_digit(0, 0)
        self.clock_display.set_digit(1, 0)
        self.clock_display.set_digit(2, 0)
        self.clock_display.set_digit(3, 0)
        self.clock_display.write_display()

        self.score_display.begin()
        self.score_display.clear()
        self.score_display.set_colon(True)
        self.score_display.set_digit(0, 0)
        self.score_display.set_digit(1, 0)
        self.score_display.set_digit(2, 0)
        self.score_display.set_digit(3, 0)
        self.score_display.write_display()

        # Test the beeper
        self.beeper.on()
        sleep(0.5)
        self.beeper.off()

        sleep(0.25)
        self.reset_hardware()

    def reset_hardware(self):
        """Reset all hardware components to the off state."""
        self.start_btn_LED.off()
        self.set_gcb_led_lights(on=False)
        self.start_btn_LED.off()
        self.beeper.off()

        self.clock_display.clear()
        self.score_display.clear()
        self.clock_display.write_display()
        self.score_display.write_display()

    def display_current_score(self):
        try:
            self.score_display.clear()
            self.score_display.set_colon(True)

            digit0 = int(self.game.black_team_score % 10)
            digit3 = int(self.game.gold_team_score % 10)
            self.score_display.set_digit(0, digit0)
            self.score_display.set_digit(3, digit3)

            self.score_display.write_display()
        except IOError:
            pass

    def display_game_time(self):
        (_, minutes, seconds) = self.game.game_clock.elapsed()

        digit0 = int(minutes / 10)
        digit1 = int(minutes % 10)
        digit2 = int(seconds / 10)
        digit3 = int(seconds % 10)

        try:
            self.clock_display.clear()
            self.clock_display.set_digit(0, digit0)
            self.clock_display.set_digit(1, digit1)
            self.clock_display.set_digit(2, digit2)
            self.clock_display.set_digit(3, digit3)
            self.clock_display.set_colon(seconds % 2)
            self.clock_display.write_display()
        except IOError:
            pass

    def undo_last_goal_scored(self, send_msg=True):
        was_goal_undone = self.game.undo_last_goal_if_allowed()

        if send_msg and was_goal_undone:
            msg = GoalUndoMessage(game_guid=self.game.game_guid, timestamp=datetime.utcnow().isoformat())
            self.rabbitmq_messenger.send_message(msg)

    def get_num_players_signed_in(self):
        """
        Returns the number of players currently signed in to the table.
        """
        n = 0

        go = self.game.gold_offense_rfid
        gd = self.game.gold_defense_rfid
        bo = self.game.black_offense_rfid
        bd = self.game.black_defense_rfid

        # Handle one player on gold
        if go == gd:
            if go is not None:
                n += 1
        # Handle two players on gold
        else:
            if go is not None:
                n += 1
            if gd is not None:
                n += 1

        # Handle one player on black
        if bo == bd:
            if bo is not None:
                n += 1
        # Handle two players on black
        else:
            if bo is not None:
                n += 1
            if bd is not None:
                n += 1

        return n

    def is_player_signed_in(self, rfid):
        """
        Returns true if the rfid given is currently signed in to the table.
        """
        if self.game.gold_offense_rfid == rfid:
            return True
        if self.game.gold_defense_rfid == rfid:
            return True
        if self.game.black_offense_rfid == rfid:
            return True
        if self.game.black_defense_rfid == rfid:
            return True

        return False

    def set_gcb_led_lights(self, on):
        if on:
            self.gcb_LED_gold_offense.on()
            self.gcb_LED_gold_defense.on()
            self.gcb_LED_black_offense.on()
            self.gcb_LED_black_defense.on()
        else:
            self.gcb_LED_gold_offense.off()
            self.gcb_LED_gold_defense.off()
            self.gcb_LED_black_offense.off()
            self.gcb_LED_black_defense.off()

    def blink_all_gcb_lights(self, on_time=0.2, off_time=0.2, n=None):
        """
        Blinks all goal-claim-button LEDs. Has the same signature as gpiozero's blink method for LEDs,
        this is just a convenience variant that controls all 4 gcbs at once, since we do this a lot throughout the app.
        Only difference between the default gpiozero version is that on & off times are default to 0.2s here, not 1s.
        """
        self.gcb_LED_gold_offense.blink(on_time=on_time, off_time=off_time, n=n, background=True)
        self.gcb_LED_gold_defense.blink(on_time=on_time, off_time=off_time, n=n, background=True)
        self.gcb_LED_black_offense.blink(on_time=on_time, off_time=off_time, n=n, background=True)
        self.gcb_LED_black_defense.blink(on_time=on_time, off_time=off_time, n=n, background=True)
