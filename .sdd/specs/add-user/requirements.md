# Requirements Document

## Project Description (Input)
Register a new user profile in the PatrimonioTech desktop application, with password-protected and isolated data storage per user.

## Introduction

This document captures the requirements for the **Add User** feature in PatrimonioTech. The feature allows a new user profile to be registered in the desktop application. Each user gets their own isolated, password-protected data storage, enabling multiple investors to use the application on the same machine without accessing each other's data.

---

## Requirements

### Requirement 1: User Registration Form

**Objective:** As an individual investor, I want a dedicated form to register a new user account, so that I can start managing my portfolio with isolated personal data.

#### Acceptance Criteria

1. When the user navigates to the user creation screen, the PatrimonioTech application shall display input fields for username, password, and password confirmation.
2. The PatrimonioTech application shall provide a **Create** button to submit the registration and a **Cancel** button to abort.
3. When the user activates the **Cancel** command, the PatrimonioTech application shall navigate back to the previous screen without creating a user.
4. When the registration is submitted successfully, the PatrimonioTech application shall navigate back to the previous screen automatically.

---

### Requirement 2: Real-Time Input Validation

**Objective:** As an individual investor, I want immediate inline feedback as I type, so that I can correct errors before submitting the form.

#### Acceptance Criteria

1. While the username field is not empty and contains non-whitespace characters, the PatrimonioTech application shall consider the username format valid.
2. If the username field is empty or contains only whitespace, the PatrimonioTech application shall consider the username format invalid and disable the **Create** command.
3. While the password field contains fewer than 8 characters, the PatrimonioTech application shall display a validation message "Senha muito curta" and disable the **Create** command.
4. If the password confirmation does not match the password field character-for-character (ordinal comparison), the PatrimonioTech application shall display a validation message "As senhas não coincidem" and disable the **Create** command.
5. The PatrimonioTech application shall enable the **Create** command only when all of the following conditions are simultaneously true: username format is valid, username is available, password meets minimum length, and password confirmation matches.

---

### Requirement 3: Real-Time Username Availability Check

**Objective:** As an individual investor, I want to know immediately whether my chosen username is already taken, so that I can pick a unique name without waiting until form submission.

#### Acceptance Criteria

1. When the username field value changes, the PatrimonioTech application shall asynchronously query the credential store to determine whether the username is already in use (case-insensitive comparison).
2. If the queried username already exists in the credential store, the PatrimonioTech application shall display a validation message "Usuário já existe" and disable the **Create** command.
3. While the username availability check is pending, the PatrimonioTech application shall keep the **Create** command disabled.
4. The PatrimonioTech application shall perform the username availability check using a case-insensitive locale-aware comparison against all stored usernames.

---

### Requirement 4: Business Rules for User Credentials

**Objective:** As an individual investor, I want the application to enforce minimum quality rules for my username and password, so that my account is identifiable and reasonably secure.

#### Acceptance Criteria

1. If the submitted username has fewer than 3 characters (after trimming whitespace), the PatrimonioTech application shall reject the registration request.
2. If the submitted password is empty or consists solely of whitespace, the PatrimonioTech application shall reject the registration request.
3. If the submitted password has fewer than 8 characters, the PatrimonioTech application shall reject the registration request.
4. The PatrimonioTech application shall enforce username and password rules independently of the UI, so that the rules apply consistently regardless of how the registration is initiated.

---

### Requirement 5: Password-Based Data Protection

**Objective:** As an individual investor, I want my portfolio data protected by my password, so that only I can access my data on this machine.

#### Acceptance Criteria

1. When a new user is registered, the PatrimonioTech application shall protect the user's data using a key derived from the user's password.
2. If the security setup fails during registration, the PatrimonioTech application shall reject the registration and not persist any user data.

---

### Requirement 6: Per-User Data Isolation

**Objective:** As an individual investor, I want my own isolated data storage created automatically upon registration, so that my portfolio data is separated from other users on the same machine.

#### Acceptance Criteria

1. When a new user is registered, the PatrimonioTech application shall provision a dedicated data store for that user.
2. If the data store cannot be created during registration, the PatrimonioTech application shall reject the registration and not persist any user credentials.

---

### Requirement 7: Credential Persistence

**Objective:** As an individual investor, I want my account to be remembered after registration, so that I can log in again on future application launches.

#### Acceptance Criteria

1. When registration succeeds, the PatrimonioTech application shall durably store the user's credentials so they are available on subsequent launches.
2. If a user with the same username already exists, the PatrimonioTech application shall reject the registration and not overwrite the existing user's credentials.

