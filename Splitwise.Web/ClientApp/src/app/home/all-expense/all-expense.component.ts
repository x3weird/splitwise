import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/user.service';

@Component({
  selector: 'app-all-expense',
  templateUrl: './all-expense.component.html',
  styleUrls: ['./all-expense.component.css']
})
export class AllExpenseComponent implements OnInit {

  expenses: any;
  //j: number[];
  //paidBy: string[];
  //paid: number[];
  //lentBy: string[];
  //lentTo: string[];
  constructor(private service: UserService) { }

  ngOnInit() {
    this.service.getExpenseList().subscribe(
      (res: any) => { console.log(res); this.expenses = res;},
      (err) => { console.log(err); }
    );

    //for (var i = 0; i < this.expenses.length; i++) {
    //  this.paid.push(0);
    //  this.j.push(0);
    //  for (var k = 0; k < this.expenses[i].expenseLedgers.length; k++) {
    //    if (this.expenses[i].expenseLedgers[i].paid > 0) {
    //      this.j.push(this.j[i] + 1);
    //      if (this.j[i] = 1) {
    //        this.paidBy[i] = this.expenses[i].expenseLedgers[k].name;
    //        this.paid[i] = this.expenses[i].expenseLedgers[k].paid;
    //      } else {
    //        this.paid[i] = this.paid + this.expenses[i].expenseLedgers[k].paid;
    //        this.paidBy[i] = this.j[i] + " people";
    //      }
    //      this.paid[i] = this.expenses[i].expenseLedgers[k].paid + this.paid;
    //    }
    //  }
    //  console.log(this.paid);
      
    //}
    console.log(this.expenses);
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
}
