#!/usr/bin/env python3

# This script is used to send test games for the purposes of developing YouFoos.
# It connects to the dev RabbitMQ instance and sends the appropriate messages to simulate a game.


import uuid
from datetime import datetime

from game_state_messenger import *


# Use some hex values which convert to known values of RFID number
# TODO This will eventually need to be updated when we change how we scan RFIds to no longer use the raw hex
go_rfid = '97 3A D3 40 90 00'
bo_rfid = '97 3A D2 C0 90 00'
gd_rfid = '97 3A D1 B0 90 00'
bd_rfid = '97 3A D0 A0 90 00'

# Dev RabbitMq url
rabbit_url = 'amqp://hgndjkaz:ywM1PfRG2sqA1_kRUI9xttVZmzRld1KN@crane.rmq.cloudamqp.com/hgndjkaz'

rabbit = RabbitMqMessenger(rabbit_url, "")

def main():
    print('Ready to simulate game...')

    # Start the game
    black_score = 0
    gold_score = 0
    print('Starting game...')
    game_guid, game_start_time = send_game_start_message()
    done = False

    while not (done):
        print('Press a number: 1 = GO score, 2 = GD score, 3 = BO score, 4 = BD score, 5 = Finalize')
        choice = input()

        if choice == '1':
            gold_score += 1
            send_goal_scored_message(game_guid, game_start_time, go_rfid)
        elif choice == '2':
            gold_score += 1
            send_goal_scored_message(game_guid, game_start_time, gd_rfid)
        elif choice == '3':
            black_score += 1
            send_goal_scored_message(game_guid, game_start_time, bo_rfid)
        elif choice == '4':
            black_score += 1
            send_goal_scored_message(game_guid, game_start_time, bd_rfid)
        elif choice == '5':
            send_game_end_message(game_guid, game_start_time, gold_score, black_score)
            done = True

    print('Exiting.')


def send_game_start_message():
    game_guid = str(uuid.uuid4())
    game_start_time_utc = datetime.utcnow()

    game_start_message = GameStartMessage(game_guid, '2V2', game_start_time_utc, go_rfid, gd_rfid, bo_rfid, bd_rfid)
    rabbit.send_message(game_start_message)

    return game_guid, game_start_time_utc


def send_goal_scored_message(game_guid, game_start_time, rfid):
    goal_time = datetime.utcnow()
    relative_time = (goal_time - game_start_time).total_seconds()
    team_scored_against = ''
    if rfid == go_rfid or rfid == gd_rfid:
        team_scored_against = 'Black'
    if rfid == bo_rfid or rfid == bd_rfid:
        team_scored_against = 'Gold'

    msg = GoalScoredMessage(game_guid, datetime.utcnow(), rfid, relative_time, team_scored_against)
    rabbit.send_message(msg)


def send_game_end_message(game_guid, game_start_time, gold_score, black_score):
    timestamp = datetime.utcnow()
    time_secs = (timestamp - game_start_time).total_seconds()
    msg = GameEndMessage(game_guid, timestamp, gold_score, black_score, time_secs)
    rabbit.send_message(msg)


if __name__ == '__main__':
    main()
