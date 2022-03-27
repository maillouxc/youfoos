import { Component, OnInit } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';

import { AuthenticationService } from '../../../_services/authentication.service';
import { UserService } from '../../../_services/user.service';

@Component({
  selector: 'gm-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  disabled = false;
  submitted = false;
  error = '';
  hide = true;
  email = new FormControl('', [Validators.required, Validators.email]);
  returnUrl: string;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private authenticationService: AuthenticationService,
    private userService: UserService
  ) {}

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/games';

    // If there is already login information saved in local storage, just use that and skip login
    if (localStorage.length > 0) {
      this.authenticationService.loadLoginInfoFromSessionStorage();
      this.router.navigate([this.returnUrl]);
    }
  }

  get f() {
    return this.loginForm.controls;
  }

  onClickForgotPassword() {
    this.submitted = true;

    if (this.f.email.invalid) {
      return;
    }

    this.disabled = true;
    this.userService
      .getForgotPassword(this.f.email.value)
      .pipe(first())
      .subscribe(
        () => {
          this.submitted = false;
          this.router.navigate(['/forgot-password', this.f.email.value]);
        },
        error => {
          this.error = error;
          this.submitted = false;
          this.disabled = false;
        }
      );
  }

  onSubmit() {
    this.submitted = true;

    if (this.loginForm.invalid) {
      return;
    }

    this.disabled = true;
    this.authenticationService
      .login(this.f.email.value, this.f.password.value)
      .pipe(first())
      .subscribe(
        () => {
          this.submitted = false;
          this.router.navigate([this.returnUrl]);
        },
        error => {
          this.error = error;
          this.submitted = false;
          this.disabled = false;
        }
      );
  }

}
