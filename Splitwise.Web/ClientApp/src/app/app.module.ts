import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { DashboardComponent } from './home/dashboard/dashboard.component';
import { ActivityComponent } from './home/activity/activity.component';
import { AllExpenseComponent } from './home/all-expense/all-expense.component';
import { LoginComponent } from './account/login/login.component';
import { CommonModule } from '@angular/common';
import { AuthGuard } from './auth/auth.guard';
import { AuthInterceptor } from './auth/auth.interceptor';
import { RegisterComponent } from './account/register/register.component';
import { AccountComponent } from './account/account.component';
import { TagInputModule } from 'ngx-chips';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FriendComponent } from './home/friend/friend.component';
import { GroupComponent } from './home/group/group.component';
import { GroupAddComponent } from './home/group/group-add/group-add.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    ActivityComponent,
    DashboardComponent,
    RegisterComponent,
    AccountComponent,
    AllExpenseComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    FriendComponent,
    GroupComponent,
    GroupAddComponent
  ],
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    TagInputModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
        { path: 'login', component: LoginComponent },
        { path: 'account', component: AccountComponent },
         { path: 'register', component: RegisterComponent },
         {path: 'home', component: HomeComponent,
           children : [
             { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard]},
             { path: 'activity', component: ActivityComponent },
             { path: 'all', component: AllExpenseComponent },
             { path: 'groups/new', component: GroupAddComponent },
             //{ path: 'friends/new', component: FriendAddComponent },
             { path: 'groups/:id', component: GroupComponent },
             { path: 'friends/:id', component: FriendComponent },
             {path: '', redirectTo: 'dashboard', pathMatch: 'full'},
             { path: '**', component: LoginComponent }
           ] },
        { path: 'activity', component: ActivityComponent, canActivate: [AuthGuard]},
        { path: 'all', component: AllExpenseComponent, canActivate: [AuthGuard]},
        {path: '', redirectTo: 'login', pathMatch: 'full'},
        { path: '**'   , component: HomeComponent  }
    ]) //{useHash:true}
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
