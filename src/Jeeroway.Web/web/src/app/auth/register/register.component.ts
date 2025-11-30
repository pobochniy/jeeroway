import {Component} from '@angular/core';
import {Router} from '@angular/router';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule} from '@angular/forms';
import {UserService} from '../../../shared/services/user.service';
import {registerFormModel} from '../../../shared/form-models/register-form.model';
import {AuthApiService} from '../../../shared/api/auth-api.service';
import {FormValidationComponent} from '../../../shared/form-validation/form-validation.component';

@Component({
  selector: 'login-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormValidationComponent],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  providers: [AuthApiService]
})
export class RegisterComponent {

  public registerForm = registerFormModel;

  constructor(private service: AuthApiService
    , private router: Router
    , private userService: UserService) {
  }

  async onSubmit() {
    Object.keys(this.registerForm.controls).forEach(key => {
      this.registerForm.get(key)?.markAsDirty();
    });

    try {
      if (this.registerForm.valid) {
        let usr = await this.service.register(this.registerForm);
        this.userService.User = usr;
        this.router.navigateByUrl('/');
      }
    } catch {
      alert('Возникли непредвиденные ошибки. Попробуйте ввести другие значения или сообщите программисту');
    }
  }
}
