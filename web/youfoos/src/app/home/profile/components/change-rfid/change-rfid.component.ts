import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { ChangeRfidRequest } from '../../../../_dtos/requests/changeRfid.request';
import { DialogData } from '../../containers/profile.component';

@Component({
  selector: 'yf-dialog-change-rfid',
  templateUrl: 'change-rfid.component.html',
  styleUrls: ['change-rfid.component.scss']
})
export class DialogChangeRfidComponent {

  constructor(public dialogRef: MatDialogRef<DialogChangeRfidComponent>,
              @Inject(MAT_DIALOG_DATA) public data: DialogData,
              private formBuilder: FormBuilder) {
    this.changeRfidForm = this.formBuilder.group({
      // Validators for the check Rfid
      Rfid: ['', [Validators.required, Validators.pattern(/^-?([0-9]\d*)?$/)]]
    });
  }

  changeRfidForm: FormGroup;
  disabled = false;
  submitted = false;
  error: string;
  userService = this.data.userService;

  get f() {
    return this.changeRfidForm.controls;
  }

  onSubmit() {
    this.error = '';
    // Disables the submit button to stop mutliclick.
    this.submitted = true;
    // stop here if form is invalid
    if (this.changeRfidForm.invalid) {
      return;
    }
    const newRfid = new ChangeRfidRequest();
    newRfid.newRfidNumber = this.f.Rfid.value;
    this.disabled = true;
    this.userService.patchChangeRfid(this.data.id , newRfid)
      .pipe(first())
      .subscribe(
        data => {
          // if something is sent back it's successful and closes the dialog
          this.dialogRef.close();
        },
        error => {
          // if it receives in error the error message is assigned to this.error and can be displayed on the html
          this.error = error;
          // If the dialog recieves an error it unlocks the submit button instead of closing
          this.disabled = false;
        });
  }

}
