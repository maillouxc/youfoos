from fsm import State
from game import Game


class IdleState(State):

    def __init__(self, fsm):
        super(IdleState, self).__init__(fsm)

    def enter(self):
        super(IdleState, self).enter()
        self.fsm.table_controller.reset_hardware()
        self.fsm.table_controller.game = None

    def execute(self):
        super(IdleState, self).execute()
        table = self.fsm.table_controller
        if table.start_button.is_pressed and not table.start_button.is_held:
            # Init a new game
            self.fsm.table_controller.game = Game()
            self.fsm.transition("to-sign-in")

    def exit(self):
        super(IdleState, self).exit()
