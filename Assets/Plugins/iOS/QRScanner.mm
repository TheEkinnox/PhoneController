#import <AVFoundation/AVFoundation.h>
#import <UIKit/UIKit.h>

@interface QRScannerDelegate : NSObject <AVCaptureMetadataOutputObjectsDelegate>
@property (nonatomic, strong) AVCaptureSession *session;
@property (nonatomic, strong) AVCaptureVideoPreviewLayer *previewLayer;
@end

@implementation QRScannerDelegate

- (id)init {
    self = [super init];
    if (self) {
        self.session = [[AVCaptureSession alloc] init];

        AVCaptureDevice *device = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
        NSError *error = nil;
        AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:device error:&error];

        if (!error && [self.session canAddInput:input]) {
            [self.session addInput:input];
        }

        AVCaptureMetadataOutput *output = [[AVCaptureMetadataOutput alloc] init];
        if ([self.session canAddOutput:output]) {
            [self.session addOutput:output];
            [output setMetadataObjectsDelegate:self queue:dispatch_get_main_queue()];
            [output setMetadataObjectTypes:@[AVMetadataObjectTypeQRCode]];
        }

        // Create preview layer for Unity view
        UIView *rootView = UIApplication.sharedApplication.keyWindow.rootViewController.view;
        self.previewLayer = [AVCaptureVideoPreviewLayer layerWithSession:self.session];
        self.previewLayer.videoGravity = AVLayerVideoGravityResizeAspectFill;
        self.previewLayer.frame = rootView.bounds;
        [rootView.layer insertSublayer:self.previewLayer atIndex:0];
    }
    return self;
}

- (void)start {
    [self.session startRunning];
}

- (void)stop {
    [self.session stopRunning];
    [self.previewLayer removeFromSuperlayer];
    self.previewLayer = nil;
}

- (void)captureOutput:(AVCaptureOutput *)output
didOutputMetadataObjects:(NSArray<__kindof AVMetadataObject *> *)metadataObjects
       fromConnection:(AVCaptureConnection *)connection
{
    for (AVMetadataObject *obj in metadataObjects) {
        if ([obj.type isEqualToString:AVMetadataObjectTypeQRCode]) {
            AVMetadataMachineReadableCodeObject *code = (AVMetadataMachineReadableCodeObject *)obj;
            NSString *value = code.stringValue;
            if (value != nil) {
                UnitySendMessage("QRScannerManager", "OnQRDetected", [value UTF8String]);
            }
        }
    }
}

@end

static QRScannerDelegate *scanner = nil;

extern "C" {
    void StartQRScanner() {
        if (!scanner) {
            scanner = [[QRScannerDelegate alloc] init];
            [scanner start];
        }
    }

    void StopQRScanner() {
        if (scanner) {
            [scanner stop];
            scanner = nil;
        }
    }
}
