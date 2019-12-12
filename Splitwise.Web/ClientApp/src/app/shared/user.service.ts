import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  readonly BaseURL = 'http://localhost:4100/api/';
  currentURL = '';

  constructor(private http: HttpClient) { }

  dashboard() {
    return this.http.get(this.BaseURL + '/expenses/dashboard');
  }

  login(formData: any) {
    return this.http.post(this.BaseURL + '/users/login', formData.value);
  }

  register(userForm: any) {
    var body = {
      FirstName: userForm.value.firstName,
      LastName: userForm.value.lastName,
      UserName: userForm.value.userName,
      Email: userForm.value.email,
      Password: userForm.value.passwordGroup.password,
      ConfirmPassword: userForm.value.passwordGroup.confirmPassword
    }
    return this.http.post(this.BaseURL + '/users/register', body);
  }

  checkLogin(): boolean {
    if (localStorage.getItem('token') != null) {
      return true;
    } else {
      return false;
    }
  }

  logOut() {
    return this.http.get(this.BaseURL + '/users/Logout');
  }

  isRegisterPage() {
    this.currentURL = window.location.href;
    if (this.currentURL == ('http://localhost:4100/register')) {
      return true;
    } else {
      return false;
    }
  }

  isLoginPage() {
    this.currentURL = window.location.href;
    if (this.currentURL == ('http://localhost:4100/login')) {
      return true;
    } else {
      return false;
    }
  }

  groupList() {
    return this.http.get(this.BaseURL + "/groups");
  }

  friendList() {
    return this.http.get(this.BaseURL + "/friends");
  }

  addExpense(expense: any) {
    return this.http.post(this.BaseURL + "/expenses/addExpense", expense);
  }

  settleUp(settleUp: any) {
    return this.http.post(this.BaseURL + "/expenses/settleUp", settleUp);
  }

  inviteFriend(friendDetails: any) {
    return this.http.post(this.BaseURL + "/friends/inviteFriend", friendDetails);
  }

  getUserDetails() {
    return this.http.get(this.BaseURL + "/users");
  }

  editUserDetails(user: any) {
    return this.http.post(this.BaseURL + "/users", user);
  }

  getExpenseList() {
    return this.http.get(this.BaseURL+"/expenses");
  }

  getFriendExpenseList(friendId: string) {
    return this.http.get(this.BaseURL + "/friends/expenseList/" + friendId);
  }

  getGroupExpenseList(groupId: string) {
    return this.http.get(this.BaseURL + "/groups/expenseList/" + groupId);
  }

  getActivities() {
    return this.http.get(this.BaseURL + "/activities");
  }

  groupAdd(groupAddForm: any) {
    return this.http.post(this.BaseURL + "/groups" , groupAddForm);
  }

  addComment(comment: any) {
    return this.http.post(this.BaseURL + "/comments", comment);
  }

  deleteComment(commentId: string) {
    return this.http.delete(this.BaseURL + "/comments/" + commentId);
  }

  deleteExpense(expenseId: string) {
    return this.http.delete(this.BaseURL + "/expenses/" + expenseId);
  }

  unDeleteExpense(expenseId: string) {
    return this.http.get(this.BaseURL + "/expenses/unDelete/" + expenseId);
  }

  getEditExpense(expenseId: string) {
    return this.http.get(this.BaseURL + "/expenses/editExpense/" + expenseId);
  }
}
