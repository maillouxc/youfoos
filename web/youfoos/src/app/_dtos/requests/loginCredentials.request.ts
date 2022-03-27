export class LoginCredentialsRequest {

  emailAddress: string;
  password: string;

  constructor(email: string, password: string) {
    this.emailAddress = email;
    this.password = password;
  }

}
