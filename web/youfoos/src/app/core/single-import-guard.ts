/**
 * This function prevents us from accidentally importing the core module more than once in the app.
 */
export function throwIfAlreadyLoaded(parentModule: any, moduleName: string) {
  if (parentModule) {
    throw new Error(`${moduleName} has already been loaded. Import the core module in AppModule only.`);
  }
}
