export enum TournamentState {
  Registration = 0,
  Seeding = 1,
  WaitingForStartTime = 2,
  InProgress = 3,
  Completed = 4,
  ErrorNotEnoughPlayers = 5,
  ErrorUnfinishedPastDeadline = 6
}
