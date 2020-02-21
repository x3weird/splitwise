import { Injectable, EventEmitter } from '@angular/core';
import { MessageService } from 'primeng/primeng';
import * as signalR from '@aspnet/signalr';
import { Expense } from '../models/expense';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private connectionPromise: Promise<void>;
  private _hubConnection: signalR.HubConnection;
  expenseReceived = new EventEmitter<Expense>();

  constructor() {
    this.createConnection();
    this.startConnection();
    this.registerOnServerEvents();
  }

  public createConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl("http://localhost:4100/MainHub", { accessTokenFactory: () => localStorage.getItem('token') })
      .build();
  }

  public startConnection() {
    this.connectionPromise = this._hubConnection.start();
    if (this.connectionPromise) {
      this.connectionPromise.then(function () {
        console.log('Connected!');
        //this._hubConnection.invoke('ExpenseNotification');
      }).catch(function (err) {
        return console.error(err.toString());
      });
    }
  }

  private registerOnServerEvents(): void {
    this._hubConnection.on("RecieveMessage", (expense: Expense) => {
      //this.messageService.add({ severity: "success", summary: "payload", detail: 'Via SignalR' });
      this.expenseReceived.emit(expense);
    });
  }


}
