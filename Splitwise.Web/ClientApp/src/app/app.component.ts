import { Component, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { MessageService } from 'primeng/components/common/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [MessageService]
})

export class AppComponent implements OnInit {

  title = 'app';
  //private connectionPromise: Promise<void>;
  constructor() { }

  ngOnInit(): void {
    //const connection = new signalR.HubConnectionBuilder()
    //  .configureLogging(signalR.LogLevel.Information)
    //  .withUrl("http://localhost:4100/MainHub", {accessTokenFactory : () => localStorage.getItem('token')})
    //  .build();

    //this.connectionPromise = connection.start();
    //if (this.connectionPromise) {
    //  this.connectionPromise.then(function () {
    //    console.log('Connected!');
    //    connection.invoke('ExpenseNotification');
    //}).catch(function (err) {
    //  return console.error(err.toString());
    //});
    //}
    //connection.on("RecieveMessage", (expense: Expense) => {
    //  this.messageService.add({ severity: "success", summary: "payload", detail: 'Via SignalR' });
    //});

    
  }
}
