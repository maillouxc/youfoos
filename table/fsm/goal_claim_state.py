from datetime import datetime

from fsm import State
from game import Game
from game_state_messenger import GoalScoredMessage
from time import time


class GoalClaimState(State):

    def __init__(self, fsm):
        super(GoalClaimState, self).__init__(fsm)
        self.was_start_button_held = True

    def enter(self):
        super(GoalClaimState, self).execute()

        # Assume true by default in case button was held on state entry
        self.was_start_button_held = True
        self.fsm.table_controller.blink_all_gcb_lights()

    def execute(self):
        super(GoalClaimState, self).enter()
        table = self.fsm.table_controller

        # Undo last goal scored when start button is held
        if table.start_button.is_held:
            if not self.was_start_button_held:
                table.undo_last_goal_scored(send_msg=False)
                self.fsm.transition("resume-game")
            # By setting to true here we prevent dupe undos
            self.was_start_button_held = True
        else:
            self.was_start_button_held = False

        # Assign player's rfid to figure out who scored (if anyone)
        scoring_player_rfid = self._get_rfid_of_player_pressing_gcb()

        # If someone scored, send a message to Rabbit and transition if needed
        if scoring_player_rfid is not None:
            # Send goal score message
            (_, mins, secs) = table.game.game_clock.elapsed()
            game_time = (mins * 60) + secs

            # We can safely assume the goals list is never empty here
            last_scoring_team = table.game.goals[-1]
            team_scored_against = None
            if last_scoring_team == Game.TEAM_GOLD:
                team_scored_against = Game.TEAM_BLACK
            else:
                team_scored_against = Game.TEAM_GOLD

            goal_score_msg = GoalScoredMessage(
                game_guid=table.game.game_guid,
                timestamp=datetime.utcnow().isoformat(),
                scoring_player_rfid=scoring_player_rfid,
                relative_timestamp=game_time,
                team_scored_against=team_scored_against)

            table.rabbitmq_messenger.send_message(goal_score_msg)

            # If we've reached the max score, go to finalizing state
            if table.game.black_team_score >= Game.MAX_SCORE or table.game.gold_team_score >= Game.MAX_SCORE:
                self.fsm.transition("finalize")
            else:
                # Set a cooldown timer to prevent dupe goals
                table.game.last_goal_time_millis = int(round(time() * 1000))

                self.fsm.transition("resume-game")

    def _get_rfid_of_player_pressing_gcb(self):
        """
        Returns the RFID card number of the player who is currently pressing their goal claim button, or None.

        If multiple people are holding their button when this is called, there is an order of precedence as follows:
        Gold offense, Gold defense, Black offense, Black defense. We don't expect that condition to happen often.
        """
        table = self.fsm.table_controller

        if table.gcb_gold_offense.is_pressed:
            return table.game.gold_offense_rfid
        elif table.gcb_gold_defense.is_pressed:
            return table.game.gold_defense_rfid
        elif table.gcb_black_offense.is_pressed:
            return table.game.black_offense_rfid
        elif table.gcb_black_defense.is_pressed:
            return table.game.black_defense_rfid
        else:
            return None

    def exit(self):
        super(GoalClaimState, self).exit()
        self.fsm.table_controller.set_gcb_led_lights(on=False)
