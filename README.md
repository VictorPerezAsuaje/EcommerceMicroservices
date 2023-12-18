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
<p>[ ✅ ] Docker support </p>
<p>[ ✅ ] RabbitMQ support </p>
<p>[  ] API Versioning</p>
<p>[  ] Feature Flag support</p>
<p>[  ] CLI Tool for service generation</p>


### WebClient
-> Status pages:
    <p>[  ] Not Found (HTTP 404)</p>
    <p>[  ] Server Error (HTTP 500)</p>
    <p>[  ] Forbidden (HTTP 401)</p>

-> Account pages:
    <p>[  ] Profile (AuthService)</p>
    <p>[  ] Order tracking (OrderService)</p>
    <p>[  ] Wishlist (OrderService)</p>

<p>[  ] Payment pages</p>

<p>[ ❓ ] Server Side Events for domain and integration events with client-specific webhooks. For instance to confirm that the order went through properly in an async fashion, allowing the client to continue using the website in the meantime. </p>

-> Administration pages:
    <p>[  ] Dashboard for order management</p>
    <p>[  ] Dashboard for inventory management</p>
    <p>[  ] Dashboard for finance management</p>
    <p>[  ] Dashboard for traffic and logging management</p>
    <p>[  ] Health & Production status testing (Watchtower)</p>
    <p>[  ] User management</p>
    <p>[  ] Coupon management</p>
    <p>[  ] Newsletter management</p>
    <p>[  ] CMS for Blog pages and service</p>

### Auth Service
<p>[ ✅ ] Login with OAuth </p>
<p>[  ] Forgot password workflow</p>

-> Roles:
    <p>[  ] Administrator</p>
    <p>[  ] Client</p>


### Mailing Service
<p>[ ✅ ] Login (internal & external) successful</p>
<p>[ ✅ ] Registration successful</p>
<p>[  ] Forgot password</p>
<p>[  ] Order placed successfuly</p>


### Order Service
<p>[  ] Wishlist workflow</p>

-> Payment workflow 
    <p>[  ] Paypal </p>
    <p>[  ] Stripe </p>


### Cart Service
-> Scheduled events inside EF:
    <p>[ ❓ ] Auto-clear cart items depending on expiration time</p>
 

### Inventory Service
<p>[ ✍ ] Pending prototype</p>


### Review Service
<p>[ ✍ ] Pending prototype</p>


### CMS Service
<p>[ ✍ ] Pending prototype</p>


### Ticketing / Support Service
<p>[ ✍ ] Pending prototype</p>


### Newsletter Service
<p>[ ✍ ] Pending prototype</p>

## Image Gallery

![Home page](/Docs/home-page.JPG)
![Shop page](/Docs/shop-page.JPG)
![Product page](/Docs/product-page.JPG)
![Login page](/Docs/login-page.JPG)
![Place order page](/Docs/place-order-page.JPG)


