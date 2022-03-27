import uuid
from time import time

from pausable_timer import PausableTimer


class Game(object):
    TEAM_BLACK = 0
    TEAM_GOLD = 1
    GAME_TYPE_1V1 = "1v1"
    GAME_TYPE_2V2 = "2v2"
    MAX_SCORE = 5

    def __init__(self):
        self.game_guid = str(uuid.uuid4())
        self.game_clock = PausableTimer()
        self.start_time_utc = None
        self.end_time_utc = None

        self.gold_offense_rfid = None
        self.gold_defense_rfid = None
        self.black_offense_rfid = None
        self.black_defense_rfid = None

        self.black_team_score = 0
        self.gold_team_score = 0
        self.goals = []
        self.same_gold = False
        self.same_black = False

        # Used to debounce goal claiming
        self.last_goal_time_millis = 0

    def on_team_scored(self, team):
        self.goals.append(team)
        if team == Game.TEAM_BLACK:
            self.black_team_score += 1
        else:
            self.gold_team_score += 1

    def undo_last_goal_if_allowed(self):
        """
        Undoes the last goal scored as long as there at least one goal to undo.

        If a goal is undone, returns True, else returns False.
        """
        if len(self.goals) > 0:
            last_scoring_team = self.goals.pop()
            if last_scoring_team == Game.TEAM_BLACK:
                self.black_team_score -= 1
            else:
                self.gold_team_score -= 1
            return True
        else:
            return False

    def is_valid_player_config(self):
        # Shorthand bool variables for which players are signed in
        go = self.gold_offense_rfid is not None
        gd = self.gold_defense_rfid is not None
        bo = self.black_offense_rfid is not None
        bd = self.black_defense_rfid is not None

        go_rfid = self.gold_offense_rfid
        gd_rfid = self.gold_defense_rfid
        bo_rfid = self.black_offense_rfid
        bd_rfid = self.black_defense_rfid

        same_gold = (go_rfid == gd_rfid) and go
        same_black = (bo_rfid == bd_rfid) and bo

        one_gold_player = (go ^ gd) or same_gold
        one_black_player = (bo ^ bd) or same_black
        two_gold_players = (go and gd) and not same_gold
        two_black_players = (bo and bd) and not same_black

        # Using the above conditions, check if the config is valid
        if one_gold_player and one_black_player:
            return True
        elif two_gold_players and two_black_players:
            return True
        else:
            return False

    def is_doubles(self):
        if self.gold_offense_rfid == self.gold_defense_rfid and self.black_offense_rfid == self.black_defense_rfid:
            return False
        elif (self.gold_offense_rfid is not None and self.gold_defense_rfid is None) or (
                self.gold_offense_rfid is None and self.gold_defense_rfid is not None) and (
                self.black_offense_rfid is not None and self.black_defense_rfid is None) or (
                self.black_offense_rfid is None and self.black_defense_rfid is not None):
            return False
        return True

    def is_goal_cooldown_passed(self):
        """
        Determines if the cooldown period between scoring goals has elapsed.

        This is useful for debouncing the goal sensors.
        """
        cooldown_time_millis = 2500
        current_time_millis = int(round(time() * 1000))
        if (current_time_millis - self.last_goal_time_millis) > cooldown_time_millis:
            return True
        return False
