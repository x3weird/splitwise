import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/user.service';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-friend',
  templateUrl: './friend.component.html',
  styleUrls: ['./friend.component.css']
})
export class FriendComponent implements OnInit {

  expenses: any;
  k: number;
  amount: number;
  paidBy: string[] = [];
  paid: number[] = [];
  lentBy: string[] = [];
  lentTo: string[] = [];
  lent: number[] = [];

  constructor(private service: UserService, private activeRoute: ActivatedRoute, private fb: FormBuilder) { }

  commentForm: FormGroup = this.fb.group({
    commentData: ['']
  });

  ngOnInit() {

    this.activeRoute.params.subscribe(params => {
      console.log(params['id']);
      this.service.getFriendExpenseList(params['id']).subscribe(
        (res: any) => {
          console.log(res);
          this.expenses = res;
          for (var i = 0; i < this.expenses.length; i++) {
            this.k = 0;
            this.amount = 0;
            var name;
            var lentAmount = 0;
            var lentName;
            for (var j = 0; j < this.expenses[i].expenseLedgers.length; j++) {
              this.expenses[i].expenseLedgers[j]
              if (this.expenses[i].expenseLedgers[j].paid > 0) {
                this.k++;
                name = this.expenses[i].expenseLedgers[j].name;
                console.log(this.expenses[i].expenseLedgers[j].name);
                this.amount = this.amount + this.expenses[i].expenseLedgers[j].paid;
              }
              if (this.expenses[i].expenseLedgers[j].owes < 0) {
                lentAmount = lentAmount + this.expenses[i].expenseLedgers[j].owes;
                lentName = this.expenses[i].expenseLedgers[j].name;
              }
            }
            if (this.k == 1) {
              this.paidBy.push(name);
              this.paid.push(this.amount);

            } else {
              this.paidBy.push(this.k + " people");
              this.paid.push(this.amount);
            }

            this.lent.push(lentAmount);
            this.lentBy.push(name);
            this.lentTo.push(lentName);

          }
        },
        (err) => { console.log(err); }
      );
    });

    
  }

  clickExpense(id: string) {
    if (document.getElementById(id).classList.contains('hide')) {
      document.getElementById(id).classList.remove('hide');
      document.getElementById(id).classList.add('show');
    }
    else {
      document.getElementById(id).classList.remove('show');
      document.getElementById(id).classList.add('hide');
    }
  }

  onSubmit(expenseId: string) {
    console.log(this.commentForm.value);
    console.log(expenseId);

    var comment = {
      Content: this.commentForm.value.commentData,
      ExpenseId: expenseId
    };
    this.service.addComment(comment).subscribe(
      (data: any) => { console.log(data); location.reload(); },
      (err: any) => { console.log(err); }
    );
  }

  deleteExpense(expenseId: string) {
    console.log("delete");
    var check = confirm("Are you sure you want to delete expense ?");
    if (check) {
      this.service.deleteExpense(expenseId).subscribe(
        (res: any) => { console.log(res); location.reload(); },
        (err) => { console.log(err); }
      );
    }
  }
}
