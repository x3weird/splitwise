
import { Component, Input, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { UserService } from '../shared/user.service';
import { NotificationService } from '../shared/notification.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Observable, of, EMPTY } from 'rxjs';
import { Notification } from '../models/notification';
import { MessageService } from 'primeng/primeng';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public validators = [this.must_be_email];
  public errorMessages = {
    'must_be_email': 'Please be sure to use a valid email format'
  };

  @Input() sidebarMenu = [];
  groupList:any ;
  friendList: any;
  inviteFriendForm: FormGroup = this.fb.group({
    email: ['', [Validators.required]],
    message: ''
  });

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

  private connectionPromise: Promise<void>;

  constructor(private service: UserService, private fb: FormBuilder, private messageService: MessageService, private notificationService: NotificationService) {
    this.subscribeToEvents();
  }

  ngOnInit(): void {

    this.service.groupList().subscribe(
      (data: any) => { this.groupList = data; console.log(data);},
      (err) => console.log(err)
    );

    this.service.friendList().subscribe(
      (data: any) => { this.friendList = data; console.log(data); },
      (err) => console.log(err)
    );
  }

  private subscribeToEvents(): void {
    this.notificationService.expenseReceived.subscribe((notification: Notification) => {
      this.messageService.add({ severity: notification.severity, summary: notification.payload, detail: notification.detail });
    })
  }

  onInviteFriend() {
    var email: string[] = [];
    if (Array.isArray(this.inviteFriendForm.value.email)) {
      for (var i = 0; i < this.inviteFriendForm.value.email.length; i++) {
        email.push(this.inviteFriendForm.value.email[i].value);
      }
    } else {
      email.push(this.inviteFriendForm.value.email);
    }
    
    
    var inviteFriend = {
      Email: email,
      Message: this.inviteFriendForm.value.message
    };
    console.log(inviteFriend);
    this.service.inviteFriend(inviteFriend).subscribe(
      (data) => { console.log(data); 
                   location.reload();
      },
      (err) => { console.log(err) }
    );
  }

  deleteFriend(friendId: string) {
    //console.log("delete - "+friendId);
    this.service.deleteFriend(friendId).subscribe(
      (data: any) => {
        console.log(data);
        location.reload();
      },
      (err) => { console.log(err); }
    );
  }

  deleteGroup(groupId: string) {
    //console.log("delete - "+friendId);
    this.service.deleteGroup(groupId).subscribe(
      (data: any) => {
        console.log(data);
        location.reload();
      },
      (err) => { console.log(err); }
    );
  }
}
