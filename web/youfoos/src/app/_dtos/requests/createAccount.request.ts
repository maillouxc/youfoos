export class CreateAccountRequest {
  emailAddress: string;
  firstAndLastName: string;
  password: string;
  rfidNumber: string;

  constructor(name: string, email: string, password: string, rfidNumber: string) {
    this.emailAddress = email;
    this.password = password;
    this.firstAndLastName = name;
    this.rfidNumber = rfidNumber;
  }
}

