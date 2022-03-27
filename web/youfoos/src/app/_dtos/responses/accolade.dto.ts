import { AccoladeConnotation } from '../enums/AccoladeConnotation';

export class AccoladeDto {
  public type: string;
  public name: string;
  public connotation: AccoladeConnotation;
  public currentValue: string;
  public userId: string;
  public entityName: string;
}
