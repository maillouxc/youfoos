class FiniteStateMachine(object):

    def __init__(self, table_controller):
        self.table_controller = table_controller
        self.states = {}
        self.transitions = {}
        self.pending_transition = None
        self.current_state = None
        self.previous_state = None

    def add_transition(self, transition_name, transition):
        self.transitions[transition_name] = transition

    def add_state(self, state_name, state):
        self.states[state_name] = state

    def set_state(self, state_name):
        self.previous_state = self.current_state
        self.current_state = self.states[state_name]
        print("Entering state: " + state_name)

    def execute(self):
        if self.pending_transition:
            self.current_state.exit()
            self.pending_transition.execute()
            self.set_state(self.pending_transition.to_state)
            self.current_state.enter()
            self.pending_transition = None

        self.current_state.execute()

    def transition(self, transition_name):
        self.pending_transition = self.transitions[transition_name]


class State(object):

    def __init__(self, fsm):
        self.fsm = fsm

    def enter(self):
        pass

    def execute(self):
        pass

    def exit(self):
        pass


class Transition(object):

    def __init__(self, to_state):
        self.to_state = to_state

    def execute(self):
        print("Transitioning...")
