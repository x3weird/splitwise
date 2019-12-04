import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';

@Component({
  selector: 'app-group-add',
  templateUrl: './group-add.component.html',
  styleUrls: ['./group-add.component.css']
})
export class GroupAddComponent implements OnInit {

  groupAddForm: FormGroup;

  get groupMember(): FormArray {
    return <FormArray>this.groupAddForm.get('groupMembers')
  }

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.groupAddForm = this.fb.group({
      groupName: '',
      simplifyDebts: false,
      groupMembers: this.fb.array([this.buildGroupMember()])
    });

  }

  buildGroupMember(): FormGroup {
    return this.fb.group({
      name: '',
      email: ''
    })
  }

  addGroupMember(): void {
    this.groupMember.push(this.buildGroupMember());
  }

  onSubmit() {
    const groupMember = this.fb.group({
      name: '',
      email: ''
    });
    this.groupAddForm.setControl('groupMembers', this.fb.array([groupMember]));
  }

}
