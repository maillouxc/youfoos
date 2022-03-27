from fsm import State
from game import Game


class GameplayState(State):

    def __init__(self, fsm):
        super(GameplayState, self).__init__(fsm)
        self.was_start_button_held = True

    def enter(self):
        super(GameplayState, self).enter()

        # Initialize to true so we can handle if already was pressed on entry
        self.was_start_button_held=True
        self._start_or_resume_game_clock()

    def execute(self):
        super(GameplayState, self).execute()
        table = self.fsm.table_controller

        # We need to figure out whether a goal was scored
        goal_scored = False

        # Determine if either goal sensor was tripped
        # We also apply a cooldown timer to debounce the sensors
        if table.game.is_goal_cooldown_passed():
            if table.gold_breakbeam.is_pressed:
                table.game.on_team_scored(Game.TEAM_BLACK)
                goal_scored = True
            if table.black_breakbeam.is_pressed:
                table.game.on_team_scored(Game.TEAM_GOLD)
                goal_scored = True

        # Always ensure the current score/time is displayed during gameplay
        table.display_current_score()
        table.display_game_time()

        # Undo last goal if start button is held
        if table.start_button.is_held:
            if not self.was_start_button_held:
                table.undo_last_goal_scored(send_msg=True)
            # By setting to true here we prevent dupe undos
            self.was_start_button_held = True
        else:
            self.was_start_button_held = False

        if goal_scored:
            self.fsm.transition("goal-scored")

    def exit(self):
        super(GameplayState, self).exit()
        self.fsm.table_controller.game.game_clock.pause()

    def _start_or_resume_game_clock(self):
        clock = self.fsm.table_controller.game.game_clock
        if not clock.is_running():
            if clock.is_paused():
                clock.unpause()
            else:
                clock.start()
