import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/user.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-friend',
  templateUrl: './friend.component.html',
  styleUrls: ['./friend.component.css']
})
export class FriendComponent implements OnInit {

  expenses: any;

  constructor(private service: UserService, private activeRoute: ActivatedRoute) { }

  ngOnInit() {

    this.activeRoute.params.subscribe(params => {
      console.log(params['id']);
      this.service.getFriendExpenseList(params['id']).subscribe(
        (res: any) => { console.log(res); this.expenses = res; },
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

}
