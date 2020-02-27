import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/user.service';

@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.css']
})
export class ActivityComponent implements OnInit {

  activities: any;

  constructor(private service: UserService) { }

  ngOnInit() {
    this.service.getActivities().subscribe(
      (res: any) => {
        console.log(res);
        res.sort(this.sortFunction);
        this.activities = res;

      },
      (err) => { console.log(err); }
    );
  }

  unDeleteExpense(expenseId: string) {
    this.service.unDeleteExpense(expenseId).subscribe(
      (res: any) => {
        console.log(res);
        location.reload();
        },
      (err) => {
        if (err.status == 409) {alert("already deleted")}
        console.log(err);
      }
    );


  }

  sortFunction(a, b) {
    var dateA = new Date(a.date).getTime();
    var dateB = new Date(b.date).getTime();
    return dateA > dateB ? -1 : 1;
  };

}
