import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl, Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';

import { UserService } from '../../../_services/user.service';
import { CreateAccountRequest } from '../../../_dtos/requests/createAccount.request';
import { MustMatch } from '../../../_validators/must-match.validator';

@Component({
  selector: 'yf-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  @Output() registered = new EventEmitter<CreateAccountRequest>();

  registerForm: FormGroup;
  disabled = false;
  submitted = false;
  error = '';
  hide = true;
  email = new FormControl('', [Validators.required, Validators.email]);

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private userService: UserService
  ) {}

  ngOnInit() {
    this.registerForm = this.formBuilder.group(
      {
        name: ['', [Validators.required, Validators.maxLength(50)]],
        email: ['', [Validators.required, Validators.email]],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(8),
            Validators.pattern('^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$')
          ]
        ],
        rfid: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
        confirmPassword: ['', Validators.required]
      },
      { validator: MustMatch('password', 'confirmPassword') }
    );
  }

  get f() {
    return this.registerForm.controls;
  }

  onSubmit() {
    this.submitted = true;

    if (this.registerForm.invalid) {
      return;
    }

    const newUser = new CreateAccountRequest(
      this.f.name.value,
      this.f.email.value,
      this.f.password.value,
      this.f.rfid.value
    );

    this.disabled = true;
    this.userService
      .registerNewUser(newUser)
      .pipe(first())
      .subscribe(
        () => {
          this.disabled = true;
          this.router.navigate(['/login']);
        },
        error => {
          console.log(error);
          this.error = error;
          this.disabled = false;
        }
      );
  }

}
