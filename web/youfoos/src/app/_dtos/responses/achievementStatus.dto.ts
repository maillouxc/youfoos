export class AchievementStatusDto {
  public userId: string;
  public name: string;
  public description: string;
  public progressPercent: number;
  public unlockedDateTime: string;
  public numQualifyingEvents: number;
  public numRequired: string;
}
