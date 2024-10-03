import { AbstractControl, ValidationErrors } from '@angular/forms';

export function passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
  const value: string = control.value || '';

  let errorMessages: string[] = [];

  if (!/[A-Z]/.test(value)) {
    errorMessages.push('Password must have at least one uppercase letter');
  }

  if (!/[a-z]/.test(value)) {
    errorMessages.push('Password must have at least one lowercase letter');
  }

  if (!/\d/.test(value)) {
    errorMessages.push('Password must have at least one number');
  }

  if (!/[!@#$%^&*()_+\./]/.test(value)) {
    errorMessages.push('Password must have at least one special character');
  }

  // If any error messages exist, return them as the 'passwordStrength' error
  if (errorMessages.length > 0) {
    return { passwordStrength: errorMessages.join(', ') };
  }

  return null;  // Return null if the password is valid
}

