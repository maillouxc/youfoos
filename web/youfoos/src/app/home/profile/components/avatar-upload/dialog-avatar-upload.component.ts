import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject, Component } from '@angular/core';

import { UserAvatarDto } from '../../../../_dtos/useravatar.dto';
import { DialogData } from '../../containers/profile.component';

@Component({
  selector: 'yf-dialog-avatar-upload',
  templateUrl: 'dialog-avatar-upload.component.html',
  styleUrls: ['dialog-avatar-upload.component.scss']
})
export class DialogAvatarUploadComponent {

  constructor(public dialogRef: MatDialogRef<DialogAvatarUploadComponent>,
              @Inject(MAT_DIALOG_DATA) public data: DialogData) { }

  userService = this.data.userService;
  imageShow: any = '';
  showAltImage = true;
  file: any;
  showError = false;
  private reader: FileReader;
  base64result: string[];
  errorMessage: string;

  onFileChanged(event) {
    this.reader = new FileReader();
    this.file = event.target.files[0];
    if (this.file) {
      if (this.file.size >= 1048576) {
        this.showError = true;
        this.errorMessage = 'File is too large for avatar';
        this.showAltImage = true;
        return;
      }
      const img = new Image();
      img.src = window.URL.createObjectURL( this.file );
      this.showError = false;
      this.reader.readAsDataURL(event.target.files[0]);
      this.reader.onload = (event) => {
        if (img.height > 300) {
          img.height = 300;
        }
        if ( img.width > 300) {
          img.width = 300;
        }
        this.showAltImage = false;
        this.imageShow = (<FileReader>event.target).result;
        if (typeof this.reader.result === 'string') {
          this.base64result = this.reader.result.split(',');
        }
      };
    }
  }
  
  uploadPhoto() {
    this.showError = false;
    if (this.file) {
      const newAvatar = new UserAvatarDto();
      newAvatar.userId = this.data.id;
      newAvatar.base64Image = this.base64result[1];
      newAvatar.mimeType = this.base64result[0].split(/[:;]/)[1];
      this.userService.putNewAvatar(this.data.id, newAvatar).subscribe(
        data => {
          this.dialogRef.close();
          this.showError = false;
        }, error => {
          this.errorMessage = 'Upload Failed';
          this.showError = true;
        }
      );
    }
  }

}
