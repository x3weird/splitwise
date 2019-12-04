import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/shared/user.service';

@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.css']
})
export class ActivityComponent implements OnInit {

  activities

  constructor(private service: UserService) { }

  ngOnInit() {
    this.service.getActivities().subscribe(
      (res: any) => { console.log(res); this.activities = res; },
      (err) => { console.log(err); }
    );
  }

}
