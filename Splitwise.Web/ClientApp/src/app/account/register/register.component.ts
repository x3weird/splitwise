import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/user.service';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { FormBuilder, Validators, AbstractControl } from '@angular/forms';

function passwordMatcher(c: AbstractControl): { [key: string]: boolean} | null {
  const passwordControl = c.get('password');
  const confirmControl = c.get('confirmPassword');

  if (passwordControl.pristine || confirmControl.pristine) {
    return null;
  }

  if (passwordControl.value === confirmControl.value) {
    return null;
  }

  return { 'match': true };
}


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  emailMessage: string;

  constructor(private service: UserService, private router: Router, private fb: FormBuilder) { }

  userForm = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(3)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    userName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    passwordGroup: this.fb.group({
      password: ['' , Validators.required],
      confirmPassword: ['' , Validators.required]
    }, {valIdator: passwordMatcher })
  });

  ngOnInit() {
    if (localStorage.getItem('token') != null) {
      this.router.navigateByUrl('/home');
    }

    const emailControl = this.userForm.get('email');
    emailControl.valueChanges.pipe(
      debounceTime(1000)
    ).subscribe(
      value => this.setMessge(emailControl)
    );
  }

  setMessge(c: AbstractControl): void {
    this.emailMessage = '';
    if (c.touched || c.dirty && c.errors) {
      if (c.errors.required) {
        this.emailMessage = 'Please enter email';
      }
      if (c.errors.email) {
        this.emailMessage = 'Enter correct email';
      }
    }
  }

  onSubmit() {
    this.service.register(this.userForm).subscribe(
      (res: any) => {
        if (res.succeeded) {
          console.log('registration successful');
          this.userForm.reset();
          this.router.navigate(['/login']);
        } else {
          res.errors.forEach(element => {
            switch (element.code) {
              case 'DuplicateUserName':
                  alert('User Already Exists');
                break;

              default:
                  alert('Error!');
                break;
            }
          });
        }
      },
      err => {
        console.log(err);
      }
    );
  }

}
