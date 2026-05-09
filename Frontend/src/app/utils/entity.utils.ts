export function getEntityBaseUrlFromEntityName(entityName: string) {
  switch (entityName) {
    case 'Guitar':
      return 'guitars';
    default:
      console.error('Entity type not handled in user contributions!');
      return 'notfound';
  }
}

export function getEntityBaseUrlFromEntityPath(entityPath: string) {
  switch (entityPath) {
    case 'guitars':
      return 'guitars';
    default:
      console.error('Entity type not handled in user contributions!');
      return 'notfound';
  }
}
