import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FriendComponent } from './friend/friend.component';
import { GroupComponent } from './group/group.component';

@NgModule({
  imports: [
    CommonModule,
  ],
  declarations: [
    FriendComponent,
    GroupComponent
  ]
})
export class HomeModule { }
