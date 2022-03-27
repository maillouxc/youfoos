from datetime import datetime
import time

from fsm import State
from game_state_messenger import GameEndMessage


class FinalizingState(State):

    def __init__(self, fsm):
        super(FinalizingState, self).__init__(fsm)
        self._start_button_press_time = None

    def enter(self):
        super(FinalizingState, self).enter()
        table = self.fsm.table_controller
        table.start_btn_LED.blink(on_time=0.5, off_time=0.5, background=True)

        # Differentiating between long and short presses is a huge pain...
        table.start_button.when_pressed = self.on_start_button_pressed
        table.start_button.when_released = self.on_start_button_released
        table.start_button.when_held = self.on_start_button_held

    def execute(self):
        super(FinalizingState, self).execute()

    # Do not use this for button actions - it is for debouncing
    def on_start_button_pressed(self):
        self._start_button_press_time = time.time()

    def on_start_button_released(self):
        # This can happen if the button was already down on entering the state
        if self._start_button_press_time is None:
            return

        held_time = time.time() - self._start_button_press_time

        # Check if it was short press - holds are handled in the other func
        if held_time < self.fsm.table_controller.start_button.hold_time:
            self.finalize_game()

    def on_start_button_held(self):
        self.fsm.table_controller.undo_last_goal_scored(send_msg=True)
        self.fsm.transition("resume-game")

    def exit(self):
        super(FinalizingState, self).exit()
        table = self.fsm.table_controller
        table.start_button.when_pressed = None
        table.start_button.when_released = None
        table.start_button.when_held = None
        table.start_btn_LED.off()

    def finalize_game(self):
        table = self.fsm.table_controller
        game = table.game
        game.end_time_utc = datetime.utcnow()

        (_, mins, secs) = game.game_clock.elapsed()
        duration_in_secs = (60 * mins) + secs

        msg = GameEndMessage(game_guid=game.game_guid,
                             timestamp=game.end_time_utc.isoformat(),
                             gold_score=game.gold_team_score,
                             black_score=game.black_team_score,
                             final_duration_secs=duration_in_secs)

        table.rabbitmq_messenger.send_message(msg)
        table.previous_game = table.game
        self.fsm.transition("to-idle")
