//
//  VSGIFSHARE.m
//  activitysheetplugin
//
//  Created by Gilbert Annthony Barouch - App Advisory on 2016.05.18
//  Copyright (c) 2016 App Advisory. All rights reserved.
//

#import "VSGIFSHARE.h"
#import <UIKit/UIKit.h>
#import <Accounts/Accounts.h>
#import <Social/Social.h>
#import <MessageUI/MessageUI.h>

#define SYSTEM_VERSION_EQUAL_TO(v)                  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedSame)
#define SYSTEM_VERSION_GREATER_THAN(v)              ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedDescending)
#define SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(v)  ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN(v)                 ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] == NSOrderedAscending)
#define SYSTEM_VERSION_LESS_THAN_OR_EQUAL_TO(v)     ([[[UIDevice currentDevice] systemVersion] compare:v options:NSNumericSearch] != NSOrderedDescending)
/* 1.1 */

@interface VSGIFSHARE (){
    UIActivityIndicatorView *spinner;
}
@end

@implementation VSGIFSHARE


- (void)_presentActivitySheetWithData :(id)data{
    
    if ( [self isVersionSupported] == NO)
        return;
    
    UIActivityViewController *av = [[UIActivityViewController alloc]initWithActivityItems:[[NSArray alloc] initWithObjects:data,nil] applicationActivities:nil];
    [[[UIApplication sharedApplication]keyWindow].rootViewController presentViewController:av animated:YES completion:^{
        [self dismissLoadingSprite];
    }];
    
    if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
    {
        av.popoverPresentationController.sourceView = [[UIApplication sharedApplication]keyWindow].rootViewController.view;
        av.popoverPresentationController.sourceRect = CGRectMake(100,100,5,5);
    }
    
    spinner = [[UIActivityIndicatorView alloc]initWithFrame: [[UIApplication sharedApplication]keyWindow].frame];
    [spinner startAnimating];
    [[[UIApplication sharedApplication]keyWindow].rootViewController.view addSubview:spinner];
}
- (void)_presentActivitySheetWithArray : (NSArray*) data{
    
    if ( [self isVersionSupported] == NO)
        return;
    
    UIActivityViewController *av = [[UIActivityViewController alloc]initWithActivityItems:data applicationActivities:nil];
    [[[UIApplication sharedApplication]keyWindow].rootViewController presentViewController:av animated:YES completion:^{
        [self dismissLoadingSprite];
    }];
    
    if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
    {
        av.popoverPresentationController.sourceView = [[UIApplication sharedApplication]keyWindow].rootViewController.view;
        av.popoverPresentationController.sourceRect = CGRectMake(100,100,5,5);
    }
    
    spinner = [[UIActivityIndicatorView alloc]initWithFrame: [[UIApplication sharedApplication]keyWindow].frame];
    [spinner startAnimating];
    [[[UIApplication sharedApplication]keyWindow].rootViewController.view addSubview:spinner];
    
}

- (void)FB_Post:(NSString *)status url:(NSString*)GIFUrl {
    
    NSLog(@"******* URL GIPHY = %@", GIFUrl);
    
    [SLComposeServiceViewController attemptRotationToDeviceOrientation];
    SLComposeViewController *fbSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeFacebook];
    
    
    NSURL *urlObject = [NSURL URLWithString:GIFUrl];
    [fbSheet addURL:urlObject];
    
    
    if(status.length > 0) {
        [fbSheet setInitialText:status];
    }
    
    UIViewController *vc =  UnityGetGLViewController();
    
    
    [vc presentViewController:fbSheet animated:YES completion:nil];
    
    
    fbSheet.completionHandler = ^(SLComposeViewControllerResult result) {
        NSArray *vComp;
        switch(result) {
                
            case SLComposeViewControllerResultCancelled:
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [vc dismissViewControllerAnimated:YES completion:nil];
                }
                
                //                UnitySendMessage("IOSGIfShareManager", "OnFacebookPostFailed", "[ISN_DataConvertor NSIntToChar:3]");
                
                break;
            case SLComposeViewControllerResultDone:
                
                vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
                if ([[vComp objectAtIndex:0] intValue] < 7) {
                    [vc dismissViewControllerAnimated:YES completion:nil];
                }
                
                //                UnitySendMessage("IOSGIfShareManager", "OnFacebookPostSuccess", "");
                break;
        }
        
    };
    
    
}

- (void)TW_PostInBackGround:(NSString *)status url:(NSString*)gifPath {
    
    
    NSLog(@"SG_SocialShare: TW_PostInBackGround has started");
    
    NSURL *url = [NSURL URLWithString:@"https://api.twitter.com/1.1/statuses/update_with_media.json"];
    NSMutableDictionary *paramater = [[NSMutableDictionary alloc] init];
    
    //set the parameter here. to see others acceptable parameters find it at twitter API here : http://bit.ly/Occe6R
    [paramater setObject:status forKey:@"status"];
    
    ACAccountStore *accountStore = [[ACAccountStore alloc] init];
    ACAccountType *accountType = [accountStore accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierTwitter];
    
    [accountStore requestAccessToAccountsWithType:accountType options:nil completion:^(BOOL granted, NSError *error) {
        if (granted == YES) {
            
            NSArray *accountsArray = [accountStore accountsWithAccountType:accountType];
            
            if ([accountsArray count] > 0) {
                ACAccount *twitterAccount = [accountsArray lastObject];
                
                SLRequest *postRequest = [SLRequest requestForServiceType:SLServiceTypeTwitter requestMethod:SLRequestMethodPOST URL:url parameters:paramater];
                
                NSData *imageData = [[NSData alloc] initWithContentsOfFile:gifPath]; // GIF89a file
                
                
                [postRequest addMultipartData:imageData withName:@"media[]" type:@"image/gif" filename:@"animated.gif"];
                
                [postRequest setAccount:twitterAccount]; // or  postRequest.account = twitterAccount;
                
                [postRequest performRequestWithHandler:^(NSData *responseData, NSHTTPURLResponse *urlResponse, NSError *error) {
                    
                    NSString *output = [NSString stringWithFormat:@"HTTP response status: %li", (long)[urlResponse statusCode]];
                    NSLog(@"SG_SocialShare: twitter post output: %@",output);
                    dispatch_async(dispatch_get_main_queue(), ^{
                        
                    });
                    
                    if([urlResponse statusCode] == 200) {
                        //                        UnitySendMessage("IOSGIfShareManager", "OnTwitterPostSuccess", "");
                        
                    } else {
                        //                        UnitySendMessage("IOSGIfShareManager", "OnTwitterPostFailed", "[ISN_DataConvertor NSIntToChar:[urlResponse statusCode]]");
                    }
                    
                }];
            } else {
                //                UnitySendMessage("IOSGIfShareManager", "OnTwitterPostFailed", "[ISN_DataConvertor NSIntToChar:1]");
            }
        } else {
            //            UnitySendMessage("IOSGIfShareManager", "OnTwitterPostFailed", "[ISN_DataConvertor NSIntToChar:2]");
        }
    }];
    
}

/**
 *  Post status to Twitter with mediaid
 *
 *  @param mediaid        media uploaded to twitter
 *  @param completion     callback on return
 */
-(void)postStatusWithMediaId:(NSArray *)arrayOfAccounts:(NSString *)mediaid:(NSString *)status withCompletion:(void(^)(NSError *error))completion {
    
    NSURL *requestURL = [[NSURL alloc] initWithString:@"https://api.twitter.com/1.1/statuses/update.json"];
    
    NSMutableDictionary *message = [[NSMutableDictionary alloc] initWithObjectsAndKeys:
                                    status,@"status",
                                    @"true",@"wrap_links",
                                    mediaid, @"media_ids",
                                    nil];
    
    // NSLog(@"MESSAGE %@ " , message);
    
    SLRequest *postRequest = [SLRequest
                              requestForServiceType:SLServiceTypeTwitter
                              requestMethod:SLRequestMethodPOST
                              URL:requestURL parameters:message];
    
    postRequest.account = [arrayOfAccounts objectAtIndex:0];
    
    [postRequest performRequestWithHandler:
     ^(NSData *responseData, NSHTTPURLResponse *urlResponse, NSError *error)
     {
         NSString *resp = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
         
         NSLog(@"RESP %@", resp);
         
         if (error) {
             NSLog(@"%@",error.description);
         }
         else {
             NSLog(@"SUCCESS");
         }
         
     }];
}

/**
 *  Upload an image to twitter to get media_id for is
 *
 *  @param img        The image (UIImage or NSData) to be uploaded
 *  @param completion
 */
- (void)sendTweetWithImage:(NSArray *)arrayOfAccounts:(NSData *)data withCompletion:(void(^)(NSString *mediaID, NSError *error))completion {
    
    NSURL *requestURL = [[NSURL alloc] initWithString:@"https://upload.twitter.com/1.1/media/upload.json"];
    
    //Get image data
    //    NSData *data = img;
    //    if ([img isKindOfClass:[UIImage class]]) {
    //        data = UIImagePNGRepresentation(img);
    //    }
    
    ACAccount *twitterAccount = [arrayOfAccounts objectAtIndex:0];
    NSLog(@"account %@", twitterAccount);
    
    
    
    SLRequest *postRequest = [SLRequest
                              requestForServiceType:SLServiceTypeTwitter
                              requestMethod:SLRequestMethodPOST
                              URL:requestURL parameters:nil];
    
    //Setup upload TW request
    [postRequest addMultipartData:data withName:@"media" type:@"image/gif" filename:@"synth.gif"];
    postRequest.account = twitterAccount;
    //Post the request to get the media ID
    [postRequest performRequestWithHandler:
     ^(NSData *responseData, NSHTTPURLResponse *urlResponse, NSError *error)
     {
         
         NSLog(@"ERR %@", error);
         if (!error && responseData) {
             
             NSString *resp = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
             NSLog(@"restp : %@",resp);
             //DLog(@"restp : %@",resp);
             if (error) {
                 NSLog(@"error :%@",error);
             }
             NSError *jsonError = nil;
             NSDictionary *json = [NSJSONSerialization JSONObjectWithData:responseData options:0 error:&jsonError];
             
             if (jsonError) {
                 error = jsonError;
             }
             
             if (completion) {
                 completion(json[@"media_id_string"],error);
             }
         }
         else {
             if (completion) {
                 completion(nil,error);
             }
             
         }
     }];
}


- (void)dismissLoadingSprite{
    NSLog(@"Dismissing loading sprite");
    [spinner stopAnimating];
    [spinner removeFromSuperview];
}

-(BOOL)isVersionSupported{
    
    if ( SYSTEM_VERSION_LESS_THAN(@"6.0") ){
        NSLog(@"This version of iOS is not supported activity sheet is only present with devices iOS 6+!");
        UIAlertView *av = [[UIAlertView alloc]initWithTitle:@"Share is not supported" message:@"Share is not supported on devices with software older than iOS 6, please update to the most current iOS software if your device is eligible to use sharing!" delegate:nil cancelButtonTitle:@"Ok" otherButtonTitles:nil, nil];
        
        [av show];
        return NO;
    }
    else{
        
        return YES;
    }
    
}

@end

extern "C"
{
    VSGIFSHARE *social;
    
    NSArray *arrayOfAccounts;
    
    void presentActivitySheetWithString(Byte *socialData,int _length){
        
        social = [[VSGIFSHARE alloc]init];
        NSUInteger n = (unsigned long) _length;
        NSLog(@"Length is %lu",(unsigned long)n);
        NSData *d = [[NSData alloc]initWithBytes:socialData length:n*sizeof(char)];
        NSLog(@"data length %lu",(unsigned long)[d length]);
        NSString *s = [[NSString alloc]initWithData:(NSData*)d encoding:NSUTF8StringEncoding];
        [social _presentActivitySheetWithData:s];
    }
    void presentActivitySheetWithImage(Byte *socialData,int _length){
        social = [[VSGIFSHARE alloc]init];
        NSUInteger n = (unsigned long) _length;
        NSLog(@"Length is %lu",(unsigned long)n);
        NSData *d = [[NSData alloc]initWithBytes:socialData length:n];
        UIImage *img = [[UIImage alloc]initWithData:d];
        [social _presentActivitySheetWithData:img];
    }
    void presentActivitySheetWithImageAndString(char *message,Byte *imgData,int _length){
        
        social = [[VSGIFSHARE alloc]init];
        
        NSUInteger n = (unsigned long)_length;
        NSData *_imgData =[[NSData alloc]initWithBytes:imgData length:n];
        //        UIImage *img = [[UIImage alloc]initWithData:_imgData];
        NSString *_message = [NSString stringWithUTF8String:message];
        NSArray *data = [[NSArray alloc]initWithObjects:_imgData,_message, nil];
        
        [social _presentActivitySheetWithArray:data];
        //        [social FB_Post:@"caca" url:_message];
        //         [social TW_PostInBackGround:@"caca" url:_message];
    }
    
    void presentActivitySheetForFacebook(char *message, char *gifURL){
        social = [[VSGIFSHARE alloc]init];
        
        
        NSString *_message = [NSString stringWithUTF8String:message];
        
        NSString *_gifURL = [NSString stringWithUTF8String:gifURL];
        
        
        
        [social FB_Post:_message url:_gifURL];
        
    }
    
    //    void presentActivitySheetForTwitter(char *message, char *gifURL){
    void presentActivitySheetForTwitter(char *message,Byte *imgData,int _length){
        
        //    void presentActivitySheetForTwitter(char* _filePath)
        //    {
        social = [[VSGIFSHARE alloc]init];
        
        //        NSString* filePath = [NSString stringWithUTF8String:gifURL];
        
        NSString* _message = [NSString stringWithUTF8String:message];
        
        
        ACAccountStore *account = [[ACAccountStore alloc] init];
        ACAccountType *accountType = [account accountTypeWithAccountTypeIdentifier:
                                      ACAccountTypeIdentifierTwitter];
        
        NSUInteger n = (unsigned long)_length;
        NSData *_imgData =[[NSData alloc]initWithBytes:imgData length:n];
        
        [account requestAccessToAccountsWithType:accountType options:nil
                                      completion:^(BOOL granted, NSError *error) {
                                          if (granted == YES)
                                          {
                                              arrayOfAccounts = [account accountsWithAccountType:accountType];
                                              NSString* filePath = [[NSBundle mainBundle] pathForResource:@"giphy"
                                                                                                   ofType:@"gif"];
                                              
                                              NSData *data = _imgData;//[NSData dataWithContentsOfFile:filePath];
                                              [social sendTweetWithImage:arrayOfAccounts:data withCompletion:^(NSString *mediaId, NSError *err) {
                                                  
                                                  NSLog(@"mediaid : %@", mediaId);
                                                  
                                                  if(err) {
                                                      NSLog(@"ERR %@", err);
                                                  } else {
                                                      
                                                      [social postStatusWithMediaId:arrayOfAccounts:mediaId :_message withCompletion:^(NSError *err){
                                                          
                                                          if(err) {
                                                              NSLog(@"ERR %@", err);
                                                          } else {
                                                              
                                                              NSLog(@"Completed");
                                                          }
                                                          
                                                      }];
                                                      
                                                  }
                                              }];
                                          }
                                          
                                      }];
        
        
    }
    
    void presentActivitySheetForWhatsapp(char *message,Byte *imgData,int _length){
        
        social = [[VSGIFSHARE alloc]init];
        
        NSUInteger n = (unsigned long)_length;
        NSData *_imgData =[[NSData alloc]initWithBytes:imgData length:n];
        //        UIImage *img = [[UIImage alloc]initWithData:_imgData];
        NSString *_message = @""; //[NSString stringWithUTF8String:message];
        NSArray *data = [[NSArray alloc]initWithObjects:_imgData,_message, nil];
        
        [social _presentActivitySheetWithArray:data];
        //        [social FB_Post:@"caca" url:_message];
        //         [social TW_PostInBackGround:@"caca" url:_message];
    }
    
}


/*
 #pragma mark - Navigation
 
 // In a storyboard-based application, you will often want to do a little preparation before navigation
 - (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender
 {
 // Get the new view controller using [segue destinationViewController].
 // Pass the selected object to the new view controller.
 }
 */


