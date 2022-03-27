import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { DialogData } from '../../containers/profile.component';
import { ChangePasswordRequest } from '../../../../_dtos/requests/changePassword.request';
import { MustMatch } from "../../../../_validators/must-match.validator";

@Component({
  selector: 'yf-dialog-change-password',
  templateUrl: 'dialog-change-password.component.html',
  styleUrls: ['dialog-change-password.component.scss']
})
export class DialogChangePasswordComponent {

  constructor(public dialogRef: MatDialogRef<DialogChangePasswordComponent>,
              @Inject(MAT_DIALOG_DATA) public data: DialogData,
              private formBuilder: FormBuilder) {
    this.changePwForm = this.formBuilder.group(
    {
      // Validators for the check passwords
      currentPw: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    },
    { validator: MustMatch('password', 'confirmPassword') });
  }

  changePwForm: FormGroup;
  disabled = false;
  submitted = false;
  error = '';
  hide = true;
  userService = this.data.userService;

  get f() {
    return this.changePwForm.controls;
  }

  onSubmit() {
    this.error = '';
    // Disables the submit button to stop mutliclick.
    this.submitted = true;

    if (this.changePwForm.invalid) {
      return;
    }

    const newPassword = new ChangePasswordRequest();
    newPassword.newPassword = this.f.password.value;
    newPassword.oldPassword = this.f.currentPw.value;
    this.disabled = true;

    this.userService.postChangePassword(this.data.id, newPassword)
      .pipe(first())
      .subscribe(
        data => {
          // if something is sent back it's successful and closes the dialog
          this.dialogRef.close();
        },

        error => {
          // if it receives in error the error message is assigned to this.error and can be displayed on the html
          this.error = error;
          // If the dialog receives an error it unlocks the submit button instead of closing
          this.disabled = false;
        });
  }

}
