# Ecommerce Microservices 

This is just a learning-oriented project for me to further understand possible microservice scenarios, difficulties and deepen my understanding on the commonly-used technologies for this type or architecture.

## TechStack

**Backend**: 
- WebClient with .NET 8 MVC
- Services with .NET 8 WebAPI
- Data Access with Entity Framework Core
- Authentication with individual accounts or Google authentication (OAuth)
- Cookie and JWT-based auth validation
- Mailing with a custom SMTP implementation

**Testing**:
- Integration tests with XUnit

**Frontend**: 
- HTML
- CSS
- JS
- HTMX
- TailwindCSS

**Misc**: 
- Internal layered folder structure following a pseudo-DDD-N tier approach (Domain/Infrastructure/Application/UI).
- Docker (dockerfile & docker-compose)
- RabbitMQ

## Version 1.0 - Feature Goals

✅ Completed
❓ Needs researching
✍ Needs to specify requirements

### General
[ ✅ ] Docker support 
[ ✅ ] RabbitMQ support 
[  ] API Versioning
[  ] Feature Flag support
[  ] CLI Tool for service generation


### WebClient
-> Status pages:
    [  ] Not Found (HTTP 404)
    [  ] Server Error (HTTP 500)
    [  ] Forbidden (HTTP 401)

-> Account pages:
    [  ] Profile (AuthService)
    [  ] Order tracking (OrderService)
    [  ] Wishlist (OrderService)

[  ] Payment pages

[ ❓ ] Server Side Events for domain and integration events with client-specific webhooks. For instance to confirm that the order went through properly in an async fashion, allowing the client to continue using the website in the meantime. 

-> Administration pages:
    [  ] Dashboard for order management
    [  ] Dashboard for inventory management
    [  ] Dashboard for finance management
    [  ] Dashboard for traffic and logging management
    [  ] Health & Production status testing (Watchtower)
    [  ] User management
    [  ] Coupon management
    [  ] Newsletter management
    [  ] CMS for Blog pages and service

### Auth Service
[ ✅ ] Login with OAuth 
[  ] Forgot password workflow

-> Roles:
    [  ] Administrator
    [  ] Client


### Mailing Service
[ ✅ ] Login (internal & external) successful
[ ✅ ] Registration successful
[  ] Forgot password
[  ] Order placed successfuly


### Order Service
[  ] Wishlist workflow

-> Payment workflow 
    [  ] Paypal 
    [  ] Stripe 


### Cart Service
-> Scheduled events inside EF:
    [ ❓ ] Auto-clear cart items depending on expiration time
 

### Inventory Service
[ ✍ ] Pending prototype


### Review Service
[ ✍ ] Pending prototype


### CMS Service
[ ✍ ] Pending prototype


### Ticketing / Support Service
[ ✍ ] Pending prototype


### Newsletter Service
[ ✍ ] Pending prototype

## Image Gallery

![Home page](/Docs/home-page.JPG)
![Shop page](/Docs/shop-page.JPG)
![Product page](/Docs/product-page.JPG)
![Login page](/Docs/login-page.JPG)
![Place order page](/Docs/place-order-page.JPG)


