# This code is sourced from Raspberry Pi forum user tom.slick
# It has been modified slightly.
# See bit.ly/2BQXZ3w for source thread.

import time


class PausableTimer(object):

    def __init__(self):
        self._hours = 0
        self._minutes = 0
        self._seconds = 0
        self._time = -1
        self._running = False
        self._paused = False

    def start(self):
        if not self._running and self._time < 0:
            self._hours = 0
            self._minutes = 0
            self._seconds = 0
            self._running = True
            self._time = time.time()

    def stop(self):
        if self._running:
            self._minutes, self._seconds = divmod(time.time() - self._time, 60)
            self._hours, self._minutes = divmod(self._minutes, 60)
        self._running = False
        self._time = -1

    def reset(self):
        self._hours = 0
        self._minutes = 0
        self._seconds = 0
        self._running = True
        self._time = time.time()

    def elapsed(self):
        """
        :return: hours, minutes, seconds
        :rtype: tuple / int / int / float
        """
        if self._running:
            self._minutes, self._seconds = divmod(time.time() - self._time, 60)
            self._hours, self._minutes = divmod(self._minutes, 60)

        return int(self._hours), int(self._minutes), float("%.2f" % self._seconds)

    def pause(self):
        if self._running and not self._paused:
            self._paused = True
            self._running = False
            self._time = time.time() - self._time
            self._minutes, self._seconds = divmod(self._time, 60)
            self._hours, self._minutes = divmod(self._minutes, 60)

    def unpause(self):
        if self._paused and self._time != -1:
            self._paused = False
            self._running = True
            self._time = time.time() - self._time

    def is_running(self):
        return self._running

    def is_paused(self):
        return self._paused
