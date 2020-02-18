import { Component, OnInit, EventEmitter, NgZone } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import {ToastrService} from 'ngx-toastr';
import { MessageService } from 'primeng/components/common/api';
import { Expense } from './models/expense';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [MessageService]
})

export class AppComponent implements OnInit {

  title = 'app';
  expenseReceived = new EventEmitter<Expense>();
  constructor(private messageService: MessageService, private _ngZone: NgZone, private toastr: ToastrService) {
    this.subscribeToEvents();
  }

  ngOnInit(): void {
    var connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:4100/MainHub", { accessTokenFactory: () => localStorage.getItem('token') })
      .build();

    //const connection = new signalR.HubConnectionBuilder()
    //  .configureLogging(signalR.LogLevel.Information)
    //  .withUrl("http://localhost:4100/MainHub", {accessTokenFactory : () => localStorage.getItem('token')})
    //  .build();

    connection.start().then(function () {
      console.log('Connected!');
    }).catch(function (err) {
      return console.error(err.toString());
    });

    connection.on("RecieveMessage", (data: any) => {
      this.expenseReceived.emit(data);
    });

    //document.getElementById("sendButton").addEventListener("click", function (event) {
    //});
    //connection.on("BroadcastMessage", (type: string, payload: string) => {
    //  this.messageService.add({ severity: type, summary: payload, detail: 'Via SignalR' });
    //});
  }

  private subscribeToEvents(): void {
    this.expenseReceived.subscribe((expense: Expense) => {
      this._ngZone.run(() => {
        this.toastr.success(expense.Description + "Added");
      })
    });
  }
}
