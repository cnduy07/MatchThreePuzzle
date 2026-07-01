# iOS And App Store Plan

## Target

Ship the game on the Apple App Store for:

- iPhone
- iPad

Design phone portrait first, then adapt to iPad. Avoid assuming the iPad is just a stretched phone.

## Unity Player Settings To Verify

Before release, verify:

- company name
- product name
- bundle identifier
- version and build number
- iOS target minimum version
- target device includes iPhone and iPad
- orientation settings match product direction
- app icon set
- launch screen
- splash screen behavior
- stripping and managed code settings
- IL2CPP configuration
- signing team and provisioning

Do not commit private signing assets.

## Device And Layout Requirements

Test these categories:

- small iPhone screen
- notched/tall iPhone
- large iPhone
- iPad
- iPad split-screen behavior if supported

Every screen must handle:

- safe area top notch
- home indicator bottom area
- readable HUD
- centered board
- no overlapping UI
- no tiny buttons
- no off-screen modal buttons
- consistent navigation between menu, level select, gameplay, pause/settings overlays, win, and lose

## Input Requirements

The game must support touch-first play:

- tap/drag adjacent swap
- clear invalid swap feedback
- no accidental double input during cascades
- no input while board is refilling
- pause/settings buttons reachable

Desktop mouse input can remain for development, but mobile input must be explicitly verified.

## App Store Metadata Checklist

Prepare:

- app name
- subtitle
- short description
- full description
- keywords
- category
- age rating
- support URL
- privacy policy URL
- marketing URL if available
- screenshots for required iPhone/iPad sizes
- app preview video if desired

Do not use screenshots that show features or art not present in the build.

## Privacy

Keep the first release privacy-light if possible.

If the game has no analytics, ads, tracking, IAP, account system, or external SDKs, document that clearly for App Store privacy labels.

If any SDK is added, update privacy docs immediately:

- analytics
- crash reporting
- ads
- attribution
- purchases
- cloud saves
- social login
- push notifications

Privacy labels must match actual data collection.

## Monetization Caution

Avoid monetization until the base gameplay loop is polished.

If monetization is added later:

- IAP must use Apple's purchase flow.
- Any randomized paid rewards must disclose odds before purchase.
- Ads require privacy review and age-rating review.
- Economy tuning must not make early gameplay feel blocked.

## Release QA

Before App Store submission:

- boot-to-menu flow test
- menu-to-level-select flow test
- level-select-to-game flow test
- pause/settings overlay test from gameplay
- settings overlay test from menu
- win/next-level flow test
- lose/retry flow test
- fresh install test
- upgrade install test
- airplane mode test
- no-network test
- app suspend/resume test
- audio interruption test
- orientation test
- memory warning behavior
- 10-minute continuous play test
- all first-session screens reviewed
- no placeholder/debug UI
- no console errors affecting gameplay

## Build Requirement Reminder

Apple requirements change. Before submission, verify current requirements on official Apple Developer documentation, especially required Xcode version, SDK version, privacy manifests, and App Store Connect metadata rules.
