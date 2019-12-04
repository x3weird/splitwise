import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/user.service';
import { FormBuilder, FormGroup, Validators, FormControl, FormArray } from '@angular/forms';
import { Observable, of, EMPTY } from 'rxjs';
import { forEach } from '@angular/router/src/utils/collection';
import { window } from 'rxjs/operators';

export interface AutoCompleteModel {
  value: any;
  display: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  public validators = [this.must_be_email];
  public errorMessages = {
    'must_be_email': 'Please be sure to use a valid email format'
  };

  groupList: any;
  friendList: any;
  

  private must_be_email(control: FormControl) {
    var EMAIL_REGEXP = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,3}$/i;
    if (control.value.length != "" && !EMAIL_REGEXP.test(control.value)) {
      return { "must_be_email": true };
    }
    return null;
  }

  public hasErrors(tag): boolean {
    const { dirty, value, valid } = tag;
    return dirty && value.item && !valid;
  }

  public onAdding(tag): Observable<string> {
    if (!this.hasErrors(tag)) { // this is tricky the this here is actualy the ngx-chips tag-input component
      console.log(`valid adding: ${tag}`);
      return of(tag);
    } else {
      console.log(`invalid cancel adding: ${tag}`);
      return EMPTY;
    }
  }

  constructor(private service: UserService, private fb: FormBuilder) { }

  loginForm: FormGroup = this.fb.group({
    email : ['', [Validators.required, Validators.email]],
    password : ['', Validators.required]
  });

  modalExpenseForm: FormGroup = this.fb.group({
    email: ['', [Validators.required]],
    description: ['', Validators.required],
    amount: ['', Validators.required],
    paidByMultiple: [''],
    paidBy: this.fb.array([this.Paid(localStorage.getItem('email'))]),
    splitOption: [''],
    expenseType: [''],
    date: [''],
    note: [''],
    group: [''],
    expenseEach: this.fb.array([this.Expense(localStorage.getItem('email'))])
  });

  modalSettleUpForm: FormGroup = this.fb.group({
    payer: [''],
    recipient: [''],
    amount: [''],
    date: [''],
    note: [''],
    group: ['']
  });

  ngOnInit() {
    this.dashboard();
    this.service.groupList().subscribe(
      (data: any) => { this.groupList = data; console.log(data); },
      (err) => console.log(err)
    );

    this.service.friendList().subscribe(
      (data: any) => { this.friendList = data; console.log(data); },
      (err) => console.log(err)
    );
  }

  Paid(email: string): FormGroup {
    return this.fb.group({
      email: email,
      amount: ''
    });
  }

  Expense(email: string): FormGroup {
    return this.fb.group({
      email: email,
      amount: ''
    });
  }
  dashboard() {
    return this.service.dashboard().subscribe(
        (data) => console.log(data),
        (err) => console.log(err)
    );
  }

  onAddExpenseModalSubmit() {
    console.log(this.modalExpenseForm.value);
    var email: string[] = [];
    email.push(localStorage.getItem('email'));
    for (var i = 0; i < this.modalExpenseForm.value.email.length; i++) {
      email.push(this.modalExpenseForm.value.email[i].value);
    }

    if (this.modalExpenseForm.value.paidByMultiple == false) {
      this.modalExpenseForm.value.paidBy[0].amount = this.modalExpenseForm.value.amount;
    }

    if (this.modalExpenseForm.value.expenseType == "Equally") {
      var amount = (this.modalExpenseForm.value.amount) / email.length;
      for (var i = 0; i < this.modalExpenseForm.value.expenseEach.length; i++) {
        this.modalExpenseForm.value.expenseEach[i].amount = amount;
      }
    }

    if (this.modalExpenseForm.value.expenseType == "Percentage") {
      for (var i = 0; i < this.modalExpenseForm.value.paidBy.length; i++) {
        var amount: number = this.modalExpenseForm.value.paidBy[i].amount;
        amount = (this.modalExpenseForm.value.expenseEach[i].amount / 100) * this.modalExpenseForm.value.amount;
        this.modalExpenseForm.value.expenseEach[i].amount = amount;
      }
    }

    var expense = {
      EmailList: email,
      GroupId: this.modalExpenseForm.value.group,
      Amount: this.modalExpenseForm.value.amount,
      ExpenseType: this.modalExpenseForm.value.expenseType,
      Description: this.modalExpenseForm.value.description,
      Note: this.modalExpenseForm.value.note,
      CreatedOn: this.modalExpenseForm.value.date,
      PaidBy: this.modalExpenseForm.value.paidBy,
      Ledger: this.modalExpenseForm.value.expenseEach,
      AddedBy: localStorage.getItem('email')
    };
    
    console.log(expense);

    this.service.addExpense(expense).subscribe(
      (res: any) => {
        console.log(res);
        location.reload();
      },
      (err) => console.log(err)
    );
  }

  onSettleUpModalSubmit() {

    this.service.settleUp(this.modalSettleUpForm.value).subscribe(
      (data: any) => {
        console.log(data);
        location.reload();
      },
      (err) => { console.log(err) }
    );
  }

  get paidBy(): FormArray {
    return <FormArray>this.modalExpenseForm.get('paidBy');
  }

  get expenseEach(): FormArray {
    return <FormArray>this.modalExpenseForm.get('expenseEach');
  }

  paidByMultiplePeople() {
    if (this.modalExpenseForm.get('paidByMultiple').value == true) {
      for (var i = 0; i < this.modalExpenseForm.get('email').value.length; i++) {
        this.paidBy.push(this.Paid(this.modalExpenseForm.get('email').value[i].value));
      }
    } else {
      for (var i = this.paidBy.length; i > 0; i--) {
          this.paidBy.removeAt(i);
      }
    }
    
  }

  expenseByMultiplePeople() {
    if (this.modalExpenseForm.get('expenseType').value != null) {
      for (var i = this.expenseEach.length; i > 0; i--) {
        this.expenseEach.removeAt(i);
      }
      for (var i = 0; i < this.modalExpenseForm.get('email').value.length; i++) {
        this.expenseEach.push(this.Expense(this.modalExpenseForm.get('email').value[i].value));
      }
    }
  }

  splitExpense() {
    if (this.modalExpenseForm.get('splitOption').value == 'Split the expense') {
      this.modalExpenseForm.get('expenseType').setValue('Equally');
      this.expenseByMultiplePeople();
    }
  }
}
