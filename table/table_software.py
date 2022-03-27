#!/usr/bin/env python3

import argparse

from table_controller import TableController
from game_state_messenger import RabbitMqMessenger
from fsm import *


def main():
    args = parse_command_line_arguments()
    rabbitmq_messenger = RabbitMqMessenger(args.rabbit_conn, "")
    table_controller = TableController(rabbitmq_messenger)
    table_state_machine = FiniteStateMachine(table_controller)

    table_controller.do_hardware_init()

    table_state_machine.add_state("gameplay", GameplayState(table_state_machine))
    table_state_machine.add_state("sign-in", SignInState(table_state_machine))
    table_state_machine.add_state("idle", IdleState(table_state_machine))
    table_state_machine.add_state("finalizing", FinalizingState(table_state_machine))
    table_state_machine.add_state("goal-claim", GoalClaimState(table_state_machine))
    table_state_machine.add_state("spot-claim", SpotClaimState(table_state_machine))

    table_state_machine.add_transition("to-sign-in", Transition("sign-in"))
    table_state_machine.add_transition("start-game", Transition("gameplay"))
    table_state_machine.add_transition("resume-game", Transition("gameplay"))
    table_state_machine.add_transition("goal-scored", Transition("goal-claim"))
    table_state_machine.add_transition("id-scanned", Transition("spot-claim"))
    table_state_machine.add_transition("finalize", Transition("finalizing"))
    table_state_machine.add_transition("to-idle", Transition("idle"))

    table_state_machine.set_state("idle")

    while True:
        table_state_machine.execute()


# This is just here in case we need it later
def parse_command_line_arguments():
    description = "The table control software for YouFoos."
    parser = argparse.ArgumentParser(description=description)
    parser.add_argument('rabbit_conn', help="The connection string for the RabbitMQ server.")
    return parser.parse_args()


if __name__ == "__main__":
    main()
