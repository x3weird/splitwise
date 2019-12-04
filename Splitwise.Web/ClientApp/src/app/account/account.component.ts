import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserService } from '../shared/user.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {

  

  constructor(private fb: FormBuilder, private service: UserService) { }

  userForm: FormGroup = this.fb.group({
    id:'',
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    number: '',
    currency: ''
  });

  user: any;

  ngOnInit() {

    this.service.getUserDetails().subscribe(
      (data: any) => {
                          this.user = data;
                          console.log(data);
                          this.updateData();
                      },
      (err) => { console.log(err) }
    );

  }


  updateData() {

    this.userForm.patchValue({
      id: this.user.id,
      firstName: this.user.firstName,
      lastName: this.user.lastName,
      email: this.user.email,
      number: this.user.number,
      password: this.user.password,
      currency: this.user.currency
    });
  }

  onSubmit() {
    console.log(this.userForm.value);
    this.service.editUserDetails(this.userForm.value).subscribe(
      (data: any) => { console.log(data); },
      (err) => { console.log(err); }
    );
  }

}
