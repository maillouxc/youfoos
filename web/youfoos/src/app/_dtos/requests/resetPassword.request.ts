export class ResetPasswordRequest {

  newPassword: string;
  passwordResetToken: string;

  constructor(newPassword, passwordResetToken) {
    this.newPassword = newPassword;
    this.passwordResetToken = passwordResetToken;
  }

}
