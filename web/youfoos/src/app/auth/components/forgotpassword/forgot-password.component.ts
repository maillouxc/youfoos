import { Component, OnInit } from '@angular/core';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';

import { UserService } from '../../../_services/user.service';
import { MustMatch } from '../../../_validators/must-match.validator';
import { ResetPasswordRequest } from '../../../_dtos/requests/resetPassword.request';

@Component({
  selector: 'yf-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {

  forgotForm: FormGroup;
  disabled = false;
  submitted = false;
  error = '';
  hide = true;
  email: string;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService
  ) {}

  get f() {
    return this.forgotForm.controls;
  }

  ngOnInit(): void {
    this.email = this.route.snapshot.params['email'];
    this.forgotForm = this.formBuilder.group(
      {
        code: ['', [Validators.required]],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', Validators.required]
      },
      { validator: MustMatch('password', 'confirmPassword') }
    );
  }

  resendCode() {
    this.userService
      .getForgotPassword(this.email)
      .pipe(first())
      .subscribe(
        () => {
          this.router.navigate(['/forgot-password', this.email]);
        },
        error => {
          this.error = error;
          this.disabled = false;
        }
      );
  }

  onSubmit() {
    this.submitted = true;
    const newPassword = new ResetPasswordRequest(
      this.f.password.value,
      this.f.code.value
    );

    if (this.forgotForm.invalid) {
      return;
    }

    this.disabled = true;
    this.userService
      .postResetPassword(this.email, newPassword)
      .pipe(first())
      .subscribe(
        () => {
          this.router.navigate(['/login']);
        },
        error => {
          this.error = error;
          this.disabled = false;
        }
      );
  }

}
