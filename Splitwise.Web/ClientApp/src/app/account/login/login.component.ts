import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from 'src/app/shared/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  constructor(private fb: FormBuilder, private service: UserService, private router: Router) { }

  loginForm: FormGroup = this.fb.group({
    email : ['', [Validators.required, Validators.email]],
    password : ['', Validators.required]
  });

  ngOnInit() {
    if (localStorage.getItem('token') != null) {
      this.router.navigateByUrl('/home/dashboard');
    }
  }

  onSubmit() {
    this.service.login(this.loginForm).subscribe(
      (res: any) => {
        console.log(res);
        localStorage.setItem('token', res.token);
        localStorage.setItem('email', res.email);
        this.router.navigateByUrl('/home/dashboard');
      },
      err => {
        if (err.status === 400) {
          alert('incorrect username or password');
        } else {
          console.log(err);
        }
      }
    );
  }
}
