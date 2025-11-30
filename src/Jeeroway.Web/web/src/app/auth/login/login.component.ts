import {Component} from '@angular/core';
import {Router} from '@angular/router';
import {ReactiveFormsModule} from '@angular/forms';
import {AuthApiService} from '../../../shared/api/auth-api.service';
import {UserService} from '../../../shared/services/user.service';
import {loginFormModel} from '../../../shared/form-models/login-form.model';
import {AlertsService} from "../../../shared/alerts/alerts.service";
import {FormValidationComponent} from '../../../shared/form-validation/form-validation.component';

@Component({
  selector: 'login-auth',
  standalone: true,
  imports: [ReactiveFormsModule, FormValidationComponent],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [AuthApiService]
})
export class LoginComponent {
  public loginForm = loginFormModel;

  constructor(private service: AuthApiService
    , private router: Router
    , private userService: UserService
    , private alerts: AlertsService) {
  }

  async onSubmit() {
    try {
      if (this.loginForm.valid) {
        this.userService.User = await this.service.login(this.loginForm);
        // this.chatService.connectionWebSocket();
        await this.router.navigateByUrl('/');
      }
    } catch (e) {
      this.alerts.push("danger", "не подходит");
    }
  }
}
