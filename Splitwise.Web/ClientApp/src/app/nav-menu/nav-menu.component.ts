import { Component } from '@angular/core';
import { UserService } from '../shared/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  constructor(private service: UserService, private router: Router) {

  }
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  account() {
    this.router.navigate(['/account']);
  }

  onLogout() {
    
    localStorage.removeItem('token');
    location.reload();
    this.service.logOut().subscribe(
      () => this.router.navigate(['/login']),
      (err) => console.log(err)
    );
  }

  register() {
    this.service.isRegisterPage();
    this.router.navigate(['/register']);
  }

  login() {
    this.router.navigateByUrl('/login');
  }

  home() {
    this.router.navigateByUrl('/home');
  }
}
