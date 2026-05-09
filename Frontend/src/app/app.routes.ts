import { Routes } from '@angular/router';
import { UserDirectoryComponent } from './components/user-directory/user-directory.component';
import { MainframeComponent } from './components/mainframe/mainframe.component';
import { EmptyComponent } from './components/empty/empty.component';
import { SignInComponent } from './components/signin/signin.component';
import { RegisterComponent } from './components/register/register.component';
import { signedInGuard } from './guards/signedIn.guard';
import { SandboxComponent } from './components/sandbox/sandbox.component';
import { ServerErrorComponent } from './components/server-error/server-error.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { LoremIpsumComponent } from './components/lorem-ipsum/lorem-ipsum.component';
import { UserProfileComponent } from './components/users/user-profile/user-profile.component';
import { adminGuard } from './guards/admin.guard';
import { UserSettingsComponent } from './components/user-settings/user-settings.component';
import { GuitarDirectoryComponent } from './components/guitar-directory/guitar-directory.component';
import { GuitarDetailsComponent } from './components/guitar-details/guitar-details.component';
import { GuitarCreateComponent } from './components/guitar-create/guitar-create.component';
import { GuitarEditComponent } from './components/guitar-edit/guitar-edit.component';
import { UserManagerComponent } from './components/admin/user-manager/user-manager.component';
import { GuitarManagerComponent } from './components/admin/guitar-manager/guitar-manager.component';
import { GuitarRevisionComponent } from './components/guitar-revision/guitar-revision.component';
import { RevisionHistoryComponent } from './components/revision-history/revision-history.component';
import { GuitarFrameComponent } from './components/guitar-frame/guitar-frame.component';
import { UserFrameComponent } from './components/user-frame/user-frame.component';
import { UserContributionsComponent } from './components/user-contributions/user-contributions.component';
import { UserRevisionsComponent } from './components/user-revisions/user-revisions.component';
import { isUserGuard } from './guards/is-user.guard';
import { GuitarRevisionEditComponent } from './components/guitar-revision-edit/guitar-revision-edit.component';

export const routes: Routes = [
  {
    path: '',
    component: MainframeComponent,
    children: [
      { path: 'guitars', component: GuitarDirectoryComponent },
      {
        path: 'guitars/create',
        component: GuitarCreateComponent,
        canActivate: [signedInGuard],
      },
      {
        path: 'guitars/:id',
        component: GuitarFrameComponent,
        children: [
          {
            path: 'revisions/:revisionId/edit',
            component: GuitarRevisionEditComponent,
          },
          {
            path: 'revisions/:revisionId1/:revisionId2',
            component: GuitarRevisionComponent,
          },
          {
            path: 'revisions/:revisionId1',
            component: GuitarRevisionComponent,
          },
          {
            path: 'revisions',
            component: RevisionHistoryComponent,
          },
          {
            path: 'edit',
            component: GuitarEditComponent,
            canActivate: [signedInGuard],
          },
          { path: '', component: GuitarDetailsComponent },
        ],
      },
      { path: 'not-found', component: NotFoundComponent },
      { path: 'sandbox', component: SandboxComponent },
      { path: 'lorem-ipsum', component: LoremIpsumComponent },
      { path: 'users', component: UserDirectoryComponent },
      {
        path: 'users/:username',
        component: UserFrameComponent,
        children: [
          {
            path: 'revisions',
            component: UserRevisionsComponent,
            canActivate: [isUserGuard],
          },
          { path: 'contributions', component: UserContributionsComponent },
          { path: '', component: UserProfileComponent },
        ],
      },
      {
        path: 'restricted',
        component: EmptyComponent,
        canActivate: [signedInGuard],
      },
      {
        path: 'settings',
        component: UserSettingsComponent,
        canActivate: [signedInGuard],
      },
    ],
  },
  // admin only routes
  {
    path: 'admin',
    runGuardsAndResolvers: 'always',
    component: MainframeComponent,
    canActivateChild: [adminGuard],
    children: [
      { path: 'guitars', component: GuitarManagerComponent },
      { path: 'users', component: UserManagerComponent },
      { path: 'server-error', component: ServerErrorComponent },
      { path: '**', component: EmptyComponent, pathMatch: 'full' },
    ],
  },
  { path: 'signin', component: SignInComponent },
  { path: 'register', component: RegisterComponent },

  { path: '**', component: EmptyComponent, pathMatch: 'full' },
];
