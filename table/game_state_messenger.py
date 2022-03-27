import pika
import jsonpickle


class RabbitMqMessenger(object):

    def __init__(self, rabbit_ip, exchange_name):
        self.rabbit_ip = rabbit_ip
        self.exchange_name = exchange_name
        
        # Establish a connection to the RabbitMQ exchange
        connection_params = pika.connection.URLParameters(rabbit_ip)
        self._connection = pika.BlockingConnection(connection_params)
        self._channel = self._connection.channel()
        self._queue = self._channel.queue_declare(queue="GameEvents", durable=True)
        
        self.connect()

    def connect(self):
        """Establishes a connection to RabbitMQ"""
        if self._connection is None or self._connection.is_closed:
            connection_params = pika.connection.URLParameters(self.rabbit_ip)
            self._connection = pika.BlockingConnection(connection_params)
            self._channel = self._connection.channel()
            self._queue = self._channel.queue_declare(queue="GameEvents", durable=True)

    def send_message(self, message):
        """
        Sends a message to the RabbitMQ server we are currently connected to.
        If the connection is not active at the moment, it will be established.
        """
        json_msg = jsonpickle.encode(message, make_refs=False, unpicklable=False)
        print("Sending RabbitMQ message: \n" + json_msg)
        try:
            self._publish(json_msg)
            print("Message sent")
        except pika.exceptions.ConnectionClosed:
            print("RabbitMQ connection wasn't open - starting one now")
            self.connect()
            print("Attempting to send the message again")
            self._publish(json_msg)

    def _publish(self, msg):
        """
        Private function to send message to RabbitMQ.
        This can fail if connection isn't alive at the time.
        It needs to be wrapped by a function that can handle reconnecting.
        On failures, pika.exceptions.ConnectionClosed will be thrown.
        """
        self._channel.basic_publish(exchange=self.exchange_name, routing_key='GameEvents', body=msg)


class GameStartMessage(object):

    def __init__(self,
                 game_guid,
                 game_type,
                 timestamp,
                 gold_offense_rfid,
                 gold_defense_rfid,
                 black_offense_rfid,
                 black_defense_rfid):
        self.type = "gameStart"
        self.game_guid = game_guid
        self.timestamp = str(timestamp) + 'Z'
        self.game_type = game_type
        self.gold_offense_rfid = gold_offense_rfid
        self.gold_defense_rfid = gold_defense_rfid
        self.black_offense_rfid = black_offense_rfid
        self.black_defense_rfid = black_defense_rfid


class GameEndMessage(object):

    def __init__(self,
                 game_guid,
                 timestamp,
                 gold_score,
                 black_score,
                 final_duration_secs):
        self.type = "gameEnd"
        self.game_guid = game_guid
        self.timestamp = str(timestamp) + 'Z'
        self.gold_score = gold_score
        self.black_score = black_score
        self.final_duration_secs = final_duration_secs


class GoalScoredMessage(object):

    def __init__(self, game_guid, timestamp, scoring_player_rfid, relative_timestamp, team_scored_against):
        self.type = "goalScored"
        self.game_guid = game_guid
        self.timestamp = str(timestamp) + 'Z'
        self.scoring_player_rfid = scoring_player_rfid
        self.relative_timestamp = relative_timestamp
        self.team_scored_against = team_scored_against


class GoalUndoMessage(object):

    def __init__(self, timestamp, game_guid):
        self.type = "goalUndo"
        self.game_guid = game_guid
        self.timestamp = str(timestamp) + 'Z'
