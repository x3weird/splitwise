import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { UserService } from 'src/app/shared/user.service';

@Component({
  selector: 'app-group-add',
  templateUrl: './group-add.component.html',
  styleUrls: ['./group-add.component.css']
})
export class GroupAddComponent implements OnInit {

  groupAddForm: FormGroup;

  get users(): FormArray {
    return <FormArray>this.groupAddForm.get('users');
  }

  constructor(private fb: FormBuilder, private service: UserService) { }

  ngOnInit() {
    this.groupAddForm = this.fb.group({
      name: '',
      simplifyDebts: false,
      users: this.fb.array([this.buildGroupMember()])
    });

  }

  buildGroupMember(): FormGroup {
    return this.fb.group({
      name: '',
      email: ''
    });
  }

  addGroupMember(): void {
    this.users.push(this.buildGroupMember());
  }

  onSubmit() {
    console.log(this.groupAddForm.value);
    this.service.groupAdd(this.groupAddForm.value).subscribe(
      (data: any) => { console.log(data); },
      (err: any) => { console.log(err); }
    );

    this.service.groupAdd(this.groupAddForm.value);
    const groupMember = this.fb.group({
      name: '',
      email: ''
    });
    this.groupAddForm.setControl('users', this.fb.array([groupMember]));
    this.groupAddForm.reset();
    location.reload();
  }
  
}
