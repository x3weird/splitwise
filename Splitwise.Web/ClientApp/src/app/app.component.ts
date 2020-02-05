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

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl("http://localhost:4100/MainHub", {accessTokenFactory : () => localStorage.getItem('token')})
      .build();

    connection.start().then(function () {
      console.log('Connected!');
    }).catch(function (err) {
      return console.error(err.toString());
    });

    connection.on("BroadcastMessage", (type: string, payload: string) => {
      this.messageService.add({ severity: type, summary: payload, detail: 'Via SignalR' });
    });
  }
}
