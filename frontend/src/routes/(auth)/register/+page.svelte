<script lang="ts">
    import { goto } from '$app/navigation';
    import { resolve } from '$app/paths';
    import { register } from '$lib/api/auth';
    import { authStore } from '$lib/stores/auth.svelte';
    import { Button } from '$lib/components/ui/button';
    import { Input } from '$lib/components/ui/input';
    import { Label } from '$lib/components/ui/label';
    import * as Card from '$lib/components/ui/card';

    let fullName = $state('');
    let username = $state('');
    let email = $state('');
    let password = $state('');
    let confirmPassword = $state('');
    let loading = $state(false);
    let errors = $state<Record<string, string[]> | null>(null);

    async function handleSubmit() {
        loading = true;
        errors = null;
        try {
            const response = await register({ fullName, username, email, password, confirmPassword });
            authStore.login(response);
            await goto(resolve('/dashboard'));
        } catch (e: unknown) {
            errors = (e as { errors: Record<string, string[]> })?.errors ?? null;
        } finally {
            loading = false;
        }
    }

    function fieldError(field: string): string | null {
        return errors?.[field]?.[0] ?? null;
    }
</script>

<Card.Root>
    <Card.Header class="text-center">
        <Card.Title class="text-2xl">Create an account</Card.Title>
        <Card.Description>Start tracking your job applications</Card.Description>
    </Card.Header>

    <Card.Content>
        <form onsubmit={handleSubmit} class="flex flex-col gap-4">
            <div class="flex flex-col gap-2">
                <Label for="fullName">Full name</Label>
                <Input
                    id="fullName"
                    type="text"
                    bind:value={fullName}
                    placeholder="John Doe"
                />
                {#if fieldError('fullName')}
                    <p class="text-destructive text-xs">{fieldError('fullName')}</p>
                {/if}
            </div>

            <div class="flex flex-col gap-2">
                <Label for="username">Username</Label>
                <Input
                    id="username"
                    type="text"
                    bind:value={username}
                    placeholder="johndoe"
                />
                {#if fieldError('username')}
                    <p class="text-destructive text-xs">{fieldError('username')}</p>
                {/if}
            </div>

            <div class="flex flex-col gap-2">
                <Label for="email">Email</Label>
                <Input
                    id="email"
                    type="email"
                    bind:value={email}
                    placeholder="you@example.com"
                />
                {#if fieldError('email')}
                    <p class="text-destructive text-xs">{fieldError('email')}</p>
                {/if}
            </div>

            <div class="flex flex-col gap-2">
                <Label for="password">Password</Label>
                <Input
                    id="password"
                    type="password"
                    bind:value={password}
                    placeholder="••••••••"
                />
                {#if fieldError('password')}
                    <p class="text-destructive text-xs">{fieldError('password')}</p>
                {/if}
            </div>

            <div class="flex flex-col gap-2">
                <Label for="confirmPassword">Confirm password</Label>
                <Input
                    id="confirmPassword"
                    type="password"
                    bind:value={confirmPassword}
                    placeholder="••••••••"
                />
                {#if fieldError('confirmPassword')}
                    <p class="text-destructive text-xs">{fieldError('confirmPassword')}</p>
                {/if}
            </div>

            <Button type="submit" disabled={loading} class="w-full">
                {loading ? 'Creating account...' : 'Create account'}
            </Button>
        </form>
    </Card.Content>

    <Card.Footer class="justify-center">
        <p class="text-muted-foreground text-sm">
            Already have an account?
            <a href={resolve('/login')} class="text-primary underline-offset-4 hover:underline">
                Sign in
            </a>
        </p>
    </Card.Footer>
</Card.Root>